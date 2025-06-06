using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DiscViewer
{
    internal static class ImageUtil
    {
        internal static BitmapImage ConvertBase64ToBitmapImage(string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
                return null;

            // 将 base64 字符串转换为字节数组
            byte[] imageBytes = Convert.FromBase64String(base64String);

            // 创建 BitmapImage 对象并加载字节数组
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


    }

    internal static class PathUtil
    {
        public static string FindFile(string hash)
        {
            var baseD = App.TargetDire;
            return System.IO.Path.Combine(baseD, App.DireInfo.Musics[hash].FileName);
        }

    }
}
