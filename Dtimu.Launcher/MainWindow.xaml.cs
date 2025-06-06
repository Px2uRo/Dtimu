using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static Dtimu.Launcher.MainWindow;
using static System.Net.Mime.MediaTypeNames;

namespace Dtimu.Launcher
{

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DetecteThread().Start();
        }

        public delegate void MyAction();
        private void Detecte()
        {

            var highestFrameworkVersion = LauncherUtils.GetHighestDotNetVersion();

            if (highestFrameworkVersion.Major >= 4 && highestFrameworkVersion.Minor >= 5)
            {

            }
            else
            {
                Dispatcher.Invoke(new MyAction(() =>
        {
            InstallCheck.IsEnabled = false;
            CTNBtn.Content = "无法继续，除非更新 .NET Framework 到 4.5 以上";
            CTNBtn.IsEnabled = false;
        }));
            }
        }
        private Thread DetecteThread() => new Thread(Detecte);

        private void InstallCheck_Checked(object sender, RoutedEventArgs e)
        {
            CTNBtn.Content = " 继续（安装 Dtimu） ";
        }

        private void InstallCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            CTNBtn.Content = " 继续（不安装 Dtimu） ";
        }

        private void CTNBtn_Click(object sender, RoutedEventArgs e)
        {
            if((bool)InstallCheck.IsChecked)
            {

            }
            else
            {

            }
            new TestWindow().Show();
        }
    }
}
