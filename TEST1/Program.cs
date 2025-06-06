using System;
using System.IO;
using System.Runtime.InteropServices;

class Program
{
    // 导入 LyricDecoder.dll 中的 qrcdecode 函数
    [DllImport(@"I:\Xiong's\MyStudio\DiscViewer\Reference\LyricDecoder.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr qrcdecode(byte[] src, int src_len);

    static void Main()
    {
        // 指定 qrc 文件的路径
        string filePath = @"D:\QQMusicCache\QQMusicLyricNew\七音阿卡莉 (Nanawoakari)_超学生 (ちょうがくせい) - メルティックヘル - 180 - メルティックヘル_qm.qrc";

        // 检查文件是否存在
        if (!File.Exists(filePath))
        {
            Console.WriteLine("文件不存在：" + filePath);
            return;
        }

        try
        {
            // 读取文件内容为字节数组
            byte[] fileData = File.ReadAllBytes(filePath);

            // 调用 qrcdecode 进行解码
            IntPtr decodedPtr = qrcdecode(fileData, fileData.Length);

            // 检查解码是否成功
            if (decodedPtr != IntPtr.Zero)
            {
                // 将解码后的内容转换为字符串
                string decodedResult = Marshal.PtrToStringAnsi(decodedPtr);
                Console.WriteLine("解码结果：\n" + decodedResult);
            }
            else
            {
                Console.WriteLine("解码失败！");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("出现异常：" + ex.Message);
        }
    }
}
