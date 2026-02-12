using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using FFmpeg.AutoGen;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Dtimu.AvaPlayer.Controls;
public unsafe class VideoControl : Control, IDisposable
{
    private WriteableBitmap? _bitmap;
    private int _width;
    private int _height;
    private CancellationTokenSource? _cts;
    private readonly object _bitmapLock = new();

    public void Play(string file)
    {
        // 如果已有播放，先停止
        Stop();

        _cts = new CancellationTokenSource();
        var ct = _cts.Token;
        Task.Run(() => DecodeLoop(file, ct), ct);
    }

    public void Stop()
    {
        if (_cts != null && !_cts.IsCancellationRequested)
        {
            _cts.Cancel();
            _cts.Dispose();
        }
        _cts = null;
    }

    private void EnsureBitmap(int width, int height, int rowBytes)
    {
        lock (_bitmapLock)
        {
            if (_bitmap == null || _width != width || _height != height)
            {
                _width = width;
                _height = height;

                // Avalonia 常用 BGRA32 (Bgra8888)
                _bitmap?.Dispose();
                _bitmap = new WriteableBitmap(
                    new PixelSize(_width, _height),
                    new Vector(96, 96),
                    PixelFormat.Bgra8888,
                    AlphaFormat.Opaque);
            }
        }
    }

    private void DisposeBitmap()
    {
        lock (_bitmapLock)
        {
            _bitmap?.Dispose();
            _bitmap = null;
        }
    }

    private void DecodeLoop(string file, CancellationToken ct)
    {
        // 初始化（如果需要） - 在新版 ffmpeg 不再需要 av_register_all()
        AVFormatContext* fmtCtx = null;
        AVCodecContext* codecCtx = null;
        SwsContext* swsCtx = null;
        AVFrame* frame = null;
        AVFrame* rgbFrame = null;
        byte* buffer = null;
        AVPacket* pkt = null;

        try
        {
            fmtCtx = ffmpeg.avformat_alloc_context();
            if (ffmpeg.avformat_open_input(&fmtCtx, file, null, null) != 0)
                return;

            if (ffmpeg.avformat_find_stream_info(fmtCtx, null) < 0)
                return;

            int streamIndex = -1;
            for (int i = 0; i < fmtCtx->nb_streams; i++)
            {
                if (fmtCtx->streams[i]->codecpar->codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO)
                {
                    streamIndex = i;
                    break;
                }
            }

            if (streamIndex == -1) // 没找到视频流
                return;

            AVCodecParameters* codecPar = fmtCtx->streams[streamIndex]->codecpar;
            AVCodec* codec = ffmpeg.avcodec_find_decoder(codecPar->codec_id);
            if (codec == null) return;

            codecCtx = ffmpeg.avcodec_alloc_context3(codec);
            if (ffmpeg.avcodec_parameters_to_context(codecCtx, codecPar) < 0) return;
            if (ffmpeg.avcodec_open2(codecCtx, codec, null) < 0) return;

            frame = ffmpeg.av_frame_alloc();
            rgbFrame = ffmpeg.av_frame_alloc();
            if (frame == null || rgbFrame == null) return;

            // 我们选择输出为 BGRA（与 Avalonia 的 Bgra8888 匹配）
            AVPixelFormat dstFmt = AVPixelFormat.AV_PIX_FMT_BGRA;

            int dstBufSize = ffmpeg.av_image_get_buffer_size(dstFmt, codecCtx->width, codecCtx->height, 1);
            buffer = (byte*)ffmpeg.av_malloc((ulong)dstBufSize);
            if (buffer == null) return;

            byte_ptrArray4 data = new byte_ptrArray4();
            int_array4 linesize = new int_array4();

            // 填充 rgbFrame->data / linesize（只需 4 个槽）
            ffmpeg.av_image_fill_arrays(ref data, ref linesize, buffer, dstFmt, codecCtx->width, codecCtx->height, 1);

            for (int i = 0; i < 4; i++)
            {
                rgbFrame->data[(uint)i] = data[(uint)i];
                rgbFrame->linesize[(uint)i] = linesize[(uint)i];
            }

            // 使用合适的 scaling flags（SWS_BILINEAR 常用）
            swsCtx = ffmpeg.sws_getContext(
                codecCtx->width, codecCtx->height, codecCtx->pix_fmt,
                codecCtx->width, codecCtx->height, dstFmt,
                0x02, null, null, null);

            if (swsCtx == null) return;

            // 准备 WriteableBitmap（行宽可能与 FFmpeg 的 linesize 不同）
            int srcLineSize = rgbFrame->linesize[0]; // bytes per line from FFmpeg
            EnsureBitmap(codecCtx->width, codecCtx->height, srcLineSize);

            pkt = ffmpeg.av_packet_alloc();
            if (pkt == null) return;

            // 读取包并解码
            while (ffmpeg.av_read_frame(fmtCtx, pkt) >= 0)
            {
                if (ct.IsCancellationRequested) break;

                if (pkt->stream_index == streamIndex)
                {
                    int sendRes = ffmpeg.avcodec_send_packet(codecCtx, pkt);
                    if (sendRes < 0)
                    {
                        // 忽略部分错误，继续读取
                        ffmpeg.av_packet_unref(pkt);
                        continue;
                    }

                    while (ffmpeg.avcodec_receive_frame(codecCtx, frame) == 0)
                    {
                        Thread.Sleep(20);
                        if (ct.IsCancellationRequested) break;

                        // 转换到 BGRA（写入 rgbFrame）
                        ffmpeg.sws_scale(
                            swsCtx,
                            frame->data,
                            frame->linesize,
                            0,
                            codecCtx->height,
                            rgbFrame->data,
                            rgbFrame->linesize);

                        // 确保 bitmap 存在且规格正确（可能首次设置）
                        EnsureBitmap(codecCtx->width, codecCtx->height, rgbFrame->linesize[0]);

                        // 把 rgbFrame->data[0] 的像素行按行拷贝到 WriteableBitmap 的 framebuffer（处理 stride 差异）
                        Dispatcher.UIThread.Post(() =>
                        {
                            lock (_bitmapLock)
                            {
                                if (_bitmap == null) return;

                                using var fb = _bitmap.Lock();
                                // fb.Address 是 IntPtr, fb.RowBytes, fb.Size.Height
                                var dstPtr = (byte*)fb.Address;
                                var dstRowBytes = fb.RowBytes;
                                var height = fb.Size.Height;
                                var srcPtr = rgbFrame->data[0];
                                var srcLine = rgbFrame->linesize[0];

                                // 按行拷贝，避免 stride 不同导致的错位/越界
                                for (int y = 0; y < height; y++)
                                {
                                    byte* srcLinePtr = srcPtr + (long)y * srcLine;
                                    byte* dstLinePtr = dstPtr + (long)y * dstRowBytes;

                                    // 拷贝 min(srcLine, dstRowBytes) 字节
                                    int copyBytes = srcLine < dstRowBytes ? srcLine : dstRowBytes;
                                    Buffer.MemoryCopy(srcLinePtr, dstLinePtr, dstRowBytes, copyBytes);
                                }

                                // 请求重绘
                                InvalidateVisual();
                            }
                        });
                    } // end receive_frame loop
                }

                ffmpeg.av_packet_unref(pkt);
            } // end read_frame loop
        }
        catch (OperationCanceledException)
        {
            // 取消播放，正常结束
        }
        catch (Exception ex)
        {
            // 记录或处理（这里不抛出）
            Console.WriteLine($"DecodeLoop error: {ex}");
        }
        finally
        {
            // 释放所有 FFmpeg 资源（按可能为 null 的顺序）
            if (pkt != null)
            {
                ffmpeg.av_packet_free(&pkt);
                pkt = null;
            }

            if (swsCtx != null)
            {
                ffmpeg.sws_freeContext(swsCtx);
                swsCtx = null;
            }

            if (rgbFrame != null)
            {
                AVFrame* temp = rgbFrame;
                ffmpeg.av_frame_free(&temp);
                rgbFrame = null;
            }

            if (frame != null)
            {
                ffmpeg.av_frame_free(&frame);
                frame = null;
            }

            if (codecCtx != null)
            {
                ffmpeg.avcodec_free_context(&codecCtx);
                codecCtx = null;
            }

            if (fmtCtx != null)
            {
                ffmpeg.avformat_close_input(&fmtCtx);
                // avformat_close_input 会 free fmtCtx
                fmtCtx = null;
            }

            // 释放 bitmap（如果你希望在播放结束后保留最后一帧，可以注释掉）
            // DisposeBitmap();
        }
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        lock (_bitmapLock)
        {
            if (_bitmap != null)
            {
                // 目标区域：控件大小
                var destRect = new Rect(0, 0, Bounds.Width, Bounds.Height);

                // 源区域：整个 Bitmap
                var sourceRect = new Rect(0, 0, _bitmap.PixelSize.Width, _bitmap.PixelSize.Height);

                // 使用 DrawImage(source, sourceRect, destRect) 以支持缩放
                context.DrawImage(_bitmap, sourceRect, destRect);
            }
        }
    }

    public void Dispose()
    {
        Stop();
        DisposeBitmap();
    }
}
