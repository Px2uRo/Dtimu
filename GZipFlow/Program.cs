using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipFlow
{
    internal class Program
    {
        const string sourceFolder = @"I:\Xiong's\MyStudio\DiscViewer\DiscViewer\bin\Release";
        static void Main(string[] args)
        {
            GenGZip();
        }
        static void TryDecom()
        {

            string tarPath = Path.Combine(Environment.CurrentDirectory, "OUTPUTS", "Dtimu_bin.tar");
            string gzipPath = tarPath + ".gz";
            string tatTemp = tarPath+".temp";
            GZipUtil.DecompressGZipFile(gzipPath, tatTemp);
            var tarOut = tarPath.Replace(".tar", "_OUTPUT");
            Directory.CreateDirectory(tarOut);
            GZipUtil.ExtractTarFile(tatTemp, tarOut);
        }
        static void GenGZip()
        {
            string tarPath = Path.Combine(Environment.CurrentDirectory, "OUTPUTS", "Dtimu_bin.tar");
            string gzipPath = tarPath + ".gz";

            // Step 1: 将文件夹打包成 .tar 文件
            GZipUtil.CreateTar(sourceFolder, tarPath);

            // Step 2: 使用 GZipStream 压缩 .tar 文件
            GZipUtil.CompressTarFile(tarPath, gzipPath);

            if (File.Exists(gzipPath))
            {
                File.Delete(tarPath);
                // 使用 explorer.exe 并选中指定的文件
                Process.Start("explorer.exe", $"/select,\"{gzipPath}\"");
            }
            else
            {
                Console.WriteLine("文件不存在: " + gzipPath);
            }
        }
    }
}
