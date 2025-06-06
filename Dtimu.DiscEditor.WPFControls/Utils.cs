using Dtimu.DiscEditor.Core;
using Dtimu.IndexSchemas;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TagLib.Ape;

namespace Dtimu.DiscEditor.WPFControls
{
    internal static class Utils
    {
        public static VistaFolderBrowserDialog FolderDia { get; internal set; } = new VistaFolderBrowserDialog();
        public static VistaSaveFileDialog SavFileDia { get; internal set; } = new VistaSaveFileDialog();

        internal static BitmapImage ConvertBase64ToBitmapImage(string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
                return null;

            // 将 base64 字符串转换为字节数组
            byte[] imageBytes = Convert.FromBase64String(base64String);



            return ConvertArrayToBitmap(imageBytes);
        }

        internal static BitmapImage ConvertArrayToBitmap(byte[] imageBytes)
        {
            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                ms.Position = 0; // 确保流的指针指向开头
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad; // 使图像可以被即时加载到内存
                bitmapImage.StreamSource = ms;

                bitmapImage.EndInit();

                bitmapImage.Freeze(); // 使 BitmapImage 可跨线程访问
            }
            return bitmapImage;
        }

        internal static object GetProjPage(string filePath)
        {
            var res = new Dtimu.DiscEditor.WPFControls.DefualtEditPage(Project.Load(filePath));
            res.WorkingPath = filePath;
            return res;
        }

        internal static string ProcessNameOfPerformancer(string ta)
        {// 使用正则表达式提取表演者信息，考虑全角和半角括号
         
            var match = Regex.Match(ta, @"[（(]([^）)]*)[）)]");
            return match.Success ? match.Groups[1].Value : ta;

        }

        internal static void SaveByteArray(byte[] bytes, string v)
        {
            using (var a = System.IO.File.Open(v,FileMode.OpenOrCreate))
            {
                foreach (var b in bytes)
                {
                    a.WriteByte(b);
                }
            }
        }
    }
}
