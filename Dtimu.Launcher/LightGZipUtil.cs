using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Dtimu.Launcher
{
    public static class StreamExtensions
    {
        public static void CopyTo(Stream source, Stream destination, int bufferSize = 81920)
        {
            if (source.CanSeek)
            {
                source.Position = 0; // 确保从流的开头读取
            }

            byte[] buffer = new byte[bufferSize];
            int bytesRead;
            while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
            {
                destination.Write(buffer, 0, bytesRead);
            }
        }
    }

    public class LightGZipUtil
    {
        // 创建 tar 文件
        public static void CreateTar(string sourceFolder, string tarPath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(tarPath));

            using (FileStream tarStream = new FileStream(tarPath, FileMode.Create, FileAccess.Write))
            {
                AddFolderToTar(tarStream, sourceFolder, "");
            }
        }

        // 添加文件夹到 tar 文件中
        private static void AddFolderToTar(FileStream tarStream, string folderPath, string baseFolder)
        {
            foreach (string filePath in Directory.GetFiles(folderPath))
            {
                string relativePath = Path.Combine(baseFolder, Path.GetFileName(filePath)).Replace("\\", "/");

                // 写入 tar 头部
                WriteTarHeader(tarStream, relativePath, new FileInfo(filePath).Length);

                // 写入文件内容
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    StreamExtensions.CopyTo(fileStream, tarStream);
                }

                // 处理 512 字节块对齐
                long paddingSize = 512 - (tarStream.Length % 512);
                if (paddingSize != 512)
                {
                    tarStream.Write(new byte[paddingSize], 0, (int)paddingSize);
                }
            }

            // 递归处理子目录
            foreach (string directory in Directory.GetDirectories(folderPath))
            {
                string relativePath = Path.Combine(baseFolder, Path.GetFileName(directory)).Replace("\\", "/") + "/";
                WriteTarHeader(tarStream, relativePath, 0); // 目录文件大小为 0
                AddFolderToTar(tarStream, directory, relativePath);
            }
        }

        // 写入 tar 头部信息
        private static void WriteTarHeader(FileStream tarStream, string name, long fileSize)
        {
            byte[] header = new byte[512];
            Encoding.ASCII.GetBytes(name.PadRight(100, '\0')).CopyTo(header, 0);
            Encoding.ASCII.GetBytes("0000777").CopyTo(header, 100); // 文件权限
            Encoding.ASCII.GetBytes("0000000").CopyTo(header, 108); // 用户 ID
            Encoding.ASCII.GetBytes("0000000").CopyTo(header, 116); // 组 ID
            Encoding.ASCII.GetBytes(Convert.ToString(fileSize, 8).PadLeft(11, '0')).CopyTo(header, 124); // 文件大小（八进制）
            int unixTimestamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            Encoding.ASCII.GetBytes(Convert.ToString(unixTimestamp, 8).PadLeft(11, '0')).CopyTo(header, 136);
            header[156] = (byte)'0'; // 文件类型（'0'表示普通文件）

            // 计算校验和
            for (int i = 148; i < 156; i++) header[i] = 32; // 校验和字段先填充空格
            int checksum = 0;
            foreach (byte b in header) checksum += b;
            Encoding.ASCII.GetBytes(Convert.ToString(checksum, 8).PadLeft(6, '0')).CopyTo(header, 148);

            tarStream.Write(header, 0, 512);
        }

        // 压缩 tar 文件为 gzip
        public static void CompressTarFile(string tarPath, string gzipPath)
        {
            using (FileStream tarFile = File.OpenRead(tarPath))
            using (FileStream gzipFile = File.Create(gzipPath))
            using (GZipStream gzipStream = new GZipStream(gzipFile, CompressionMode.Compress))
            {
                StreamExtensions.CopyTo(tarFile, gzipStream);
            }
        }

        // 解压 tar.gz 文件
        public static void DecompressTarGZipFileFromStream(Stream gzFileStream, string extractFolder)
        {
            using (GZipStream gzipStream = new GZipStream(gzFileStream, CompressionMode.Decompress))
            {
                ExtractTarFileFromStream(gzipStream, extractFolder);
            }
        }

        // 从 tar 流中解压文件
        public static void ExtractTarFileFromStream(Stream tarStream, string extractFolderPath)
        {
            byte[] header = new byte[512];

            while (tarStream.Read(header, 0, 512) > 0)
            {
                string fileName = Encoding.ASCII.GetString(header, 0, 100).Trim('\0');
                if (string.IsNullOrEmpty(fileName)) break;

                string sizeString = Encoding.ASCII.GetString(header, 124, 12).Trim('\0').Trim();
                long fileSize = Convert.ToInt64(sizeString, 8);

                string outputPath = Path.Combine(extractFolderPath, fileName);

                if (fileName.EndsWith("/"))
                {
                    Directory.CreateDirectory(outputPath);
                }
                else
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
                    using (FileStream outputFileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                    {
                        byte[] buffer = new byte[4096];
                        long remainingBytes = fileSize;

                        while (remainingBytes > 0)
                        {
                            int bytesToRead = remainingBytes > buffer.Length ? buffer.Length : (int)remainingBytes;
                            int bytesRead = tarStream.Read(buffer, 0, bytesToRead);
                            if (bytesRead == 0) break;

                            outputFileStream.Write(buffer, 0, bytesRead);
                            remainingBytes -= bytesRead;
                        }
                    }
                }

                // 跳过文件内容后的填充字节
                long padding = (512 - (fileSize % 512)) % 512;
                if (padding > 0 && tarStream.CanSeek)
                {
                    tarStream.Seek(padding, SeekOrigin.Current);
                }
                else if (padding > 0)
                {
                    byte[] skipBuffer = new byte[padding];
                    tarStream.Read(skipBuffer, 0, (int)padding);
                }
            }
        }
    }
}
