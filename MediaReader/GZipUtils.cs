using System;
using System.IO;
using System.IO.Compression;
using System.Text;

public class GZipUtil
{

    public static void CreateTar(string sourceFolder, string tarPath)
    {
        // 确保目标目录存在
        Directory.CreateDirectory(Path.GetDirectoryName(tarPath));

        using (FileStream tarStream = new FileStream(tarPath, FileMode.Create, FileAccess.Write))
        {
            AddFolderToTar(tarStream, sourceFolder, "");
        }
    }

    public static void AddFolderToTar(FileStream tarStream, string folderPath, string baseFolder)
    {
        foreach (string filePath in Directory.GetFiles(folderPath))
        {
            string relativePath = Path.Combine(baseFolder, Path.GetFileName(filePath)).Replace("\\", "/");

            // 写入 tar 头部
            WriteTarHeader(tarStream, relativePath, new FileInfo(filePath).Length);

            // 写入文件内容
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                fileStream.CopyTo(tarStream);
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
        header[154] = 0; // null 终止符
        header[155] = 32; // 空格

        tarStream.Write(header, 0, 512);
    }
    public static void CompressTarFile(string tarPath, string gzipPath)
    {
        using (FileStream tarFile = File.OpenRead(tarPath))
        {
            using (FileStream gzipFile = File.Create(gzipPath))
            {
                using (GZipStream gzipStream = new GZipStream(gzipFile, CompressionMode.Compress))
                {
                    tarFile.CopyTo(gzipStream);
                }
            }
        }
    }
    public static void DecompressGZipFile(string gzFilePath, string tarFilePath)
    {
        using (FileStream gzFileStream = new FileStream(gzFilePath, FileMode.Open, FileAccess.Read))
        using (GZipStream gzipStream = new GZipStream(gzFileStream, CompressionMode.Decompress))
        using (FileStream tarFileStream = new FileStream(tarFilePath, FileMode.Create, FileAccess.Write))
        {
            gzipStream.CopyTo(tarFileStream);
        }
    }

    public static void ExtractTarFile(string tarFilePath, string extractFolderPath)
    {
        using (FileStream tarStream = new FileStream(tarFilePath, FileMode.Open, FileAccess.Read))
        {
            byte[] header = new byte[512];

            while (tarStream.Read(header, 0, 512) > 0)
            {
                // 读取文件名（最多100字节）
                string fileName = Encoding.ASCII.GetString(header, 0, 100).Trim('\0');
                if (string.IsNullOrEmpty(fileName)) break; // 如果没有文件名，说明已到结尾

                // 读取文件大小（八进制字符串，文件大小在偏移量124到135之间）
                string sizeString = Encoding.ASCII.GetString(header, 124, 12).Trim('\0').Trim();
                long fileSize = Convert.ToInt64(sizeString, 8);

                // 输出路径
                string outputPath = Path.Combine(extractFolderPath, fileName);

                if (fileName.EndsWith("/")) // 如果是文件夹
                {
                    Directory.CreateDirectory(outputPath);
                }
                else // 如果是文件
                {
                    // 确保文件夹存在
                    Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

                    using (FileStream outputFileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                    {
                        byte[] buffer = new byte[4096];
                        long remainingBytes = fileSize;

                        // 读取文件内容
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
                if (padding > 0)
                {
                    tarStream.Seek(padding, SeekOrigin.Current);
                }
            }
        }
    }
}
