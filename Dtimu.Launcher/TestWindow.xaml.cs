using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace Dtimu.Launcher
{
    /// <summary>
    /// TestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TestWindow : Window
    {
        public TestWindow()
        {
            InitializeComponent();
            VersionInfo.Text ="启动器版本："+ System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
#if !DEBUG
            try
            {
                var a = Path.Combine(Environment.CurrentDirectory, "DiskInfo.txt");
                var aa = File.ReadAllText(a);
                var aaa = InfoInstance.Parse(aa);
                this.DataContext = aaa;
            }
            catch (Exception ex)
            {
                MessageBox.Show("没有找到信息文件，程序已退出！");
                this.Close();
            }
#endif
        }

        private void EnterDTIMU_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var dtm = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"Dtimu_bin");
            Directory.CreateDirectory(dtm);

            var pn = Path.Combine(dtm, "DiscViewer.exe");
            if (!File.Exists(pn))
            {
                using (var ms = new MemoryStream(Properties.Resources.Dtimu_bin_tar))
                {
                    LightGZipUtil.DecompressTarGZipFileFromStream(ms, dtm);
                }
            }

            Process.Start(pn,Environment.CurrentDirectory);
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //ExplorerHelper.EmbedExplorer(this, EXPLORE);
        }

        private void InstallNET_Click(object sender, RoutedEventArgs e)
        {
            var t = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var tt = Path.Combine(t,"NET40.exe" );
            LightGZipUtil.DecompressTarGZipFileFromStream(new MemoryStream(Properties.Resources.NET40_tar),t);

            Process.Start(new ProcessStartInfo
            {
                FileName = tt,
                UseShellExecute = false
            });
        }

        private void Explorer_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = Environment.CurrentDirectory, // 你可以在这里更改路径
                UseShellExecute = true
            });

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ClearDtimu_Click(object sender, RoutedEventArgs e)
        {

            var dtm = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dtimu_bin");
            new DirectoryInfo(dtm).Delete(true);
        }
    }
}
