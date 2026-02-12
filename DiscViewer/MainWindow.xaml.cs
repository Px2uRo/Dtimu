using DiscViewer.Views;
using System;
using System.Collections.Generic;
using System.Linq;
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
using static DiscViewer.Views.PlayingBar;

namespace DiscViewer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            PlayingBar.Current.Played += Current_Played;
            PlayingBar.Current.HiddenPropChanged += Current_HiddenPropChanged;
        }

        private void Current_HiddenPropChanged(object sender, EventArgs e)
        {
            var bar = sender as PlayingBar;
            if (bar.Hidden)
            {
                new Thread(() => {
                    Thread.Sleep(300);
                    Dispatcher.Invoke(new MyAction(() =>
                    {
                        if (bar.Hidden)
                        {
                            BottonGrid.Height = 15;
                        }
                    }));
                }).Start();

            }
            else
            {
                BottonGrid.Height = double.NaN;
            }
        }

        private void Current_Played(object sender, RoutedEventArgs e)
        {
            BottonGrid.Background = new SolidColorBrush(Colors.Transparent);
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (PlayingBar.Current.Stopped)
            {
                BottonGrid.Background = null;
                return;
            }
            PlayingBar.Show();
        }

        private void BottonGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            PlayingBar.Hide();
        }

        internal void GoFullScreen()
        {
            this.WindowStyle = WindowStyle.None;        // 去除边框
            this.ResizeMode = ResizeMode.NoResize;     // 禁止调整大小
            this.WindowState = WindowState.Maximized;  // 最大化窗口
        }

        internal void ExitFullScreen()
        {
            this.WindowStyle = WindowStyle.SingleBorderWindow; // 恢复边框
            this.ResizeMode = ResizeMode.CanResize;            // 允许调整大小
            this.WindowState = WindowState.Normal;            // 恢复正常状态
        }
    }
}
