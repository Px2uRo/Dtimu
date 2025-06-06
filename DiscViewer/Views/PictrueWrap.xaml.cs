using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiscViewer.Views
{
    /// <summary>
    /// PictrueWrap.xaml 的交互逻辑
    /// </summary>
    public partial class PictrueWrap : UserControl
    {
        public static UIElement UI { get; internal set; } = new PictrueWrap();

        public PictrueWrap()
        {
            InitializeComponent();
            LoadPictures();
        }

        private void LoadPictures()
        {
            var groupedImages = App.DireInfo.Images.Values
    .GroupBy(img => img.ShotTime.Date) 
    .OrderBy(group => group.Key); 

            foreach (var group in groupedImages)
            {
                // 添加日期标题
                var dateText = new TextBlock
                {
                    Text = group.Key.ToString("yyyy-MM-dd"), // 格式化日期
                    FontWeight = FontWeights.Bold,
                    FontSize = 20d
                };
                MyStack.Children.Add(dateText);

            // 添加 WrapPanel
            var wrapPanel = new WrapPanel();

                foreach (var img in group)
                {
                    var ima = new System.Windows.Controls.Image();
                    ima.Height = 100;

                    var array = Convert.FromBase64String(App.DireInfo.Pictures[img.SmallPic]);
                    using (MemoryStream ms = new MemoryStream(array))
                    {
                        ms.Position = 0;
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = ms;
                        bitmapImage.EndInit();
                        bitmapImage.Freeze();
                        ima.Source = bitmapImage;
                    }

                    ima.DataContext = img;
                    ima.MouseDown += Ima_MouseDown;
                    wrapPanel.Children.Add(ima);
                }

                MyStack.Children.Add(wrapPanel);
            }
        }

        private void Ima_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var dc = (sender as FrameworkElement).DataContext as Dtimu.Core.Image;

            string imagePath = System.IO.Path.Combine(App.TargetDire, dc.FilePath);

            // 获取操作系统版本
            string windowsVersion = DiscViewer.OSVersionHelper.GetWindowsVersion();

            try
            {
                if (windowsVersion.Contains("Windows 10"))
                {
                    OpenWithDefaultProgram(imagePath);
                }
                else if (windowsVersion.Contains("Windows 7") || windowsVersion.Contains("Windows 8"))
                {
                    OpenWithWindowsPhotoViewer(imagePath);
                }
                else if (windowsVersion.Contains("Windows XP"))
                {
                    OpenWithWindowsXPViewer(imagePath);
                }
                else
                {
                    Console.WriteLine("不支持的操作系统版本！");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("打开图片时发生错误：" + ex.Message);
            }
        }
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);

        public void OpenImageWithShellExecute(string filePath)
        {
            // 获取文件所在的目录
            string directory = System.IO.Path.GetDirectoryName(filePath);

            // 调用 ShellExecute 打开 JPG 文件，文件路径和所在目录被传入
            ShellExecute(IntPtr.Zero, "open", filePath, null, directory, 1);
        }
        private void OpenWithDefaultProgram(string imagePath)
        {
            OpenImageWithShellExecute(imagePath);
        }

        private void OpenWithWindowsPhotoViewer(string imagePath)
        {
            Process.Start(imagePath); // Windows 7 默认支持
        }

        private void OpenWithWindowsXPViewer(string imagePath)
        {
                Process.Start("rundll32.exe", $"\"C:\\WINDOWS\\system32\\shimgvw.dll\",ImageView_Fullscreen {imagePath}");
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {

            var mw = App.Current.MainWindow as MainWindow;
            mw.mainBorder.Child = null;
            mw.mainBorder.Child = StartPage.UI;
        }
    }
}
