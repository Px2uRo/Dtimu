using System;
using System.Collections.Generic;
using System.Linq;
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
    /// StartPage.xaml 的交互逻辑
    /// </summary>
    public partial class StartPage : UserControl
    {
        public static UIElement UI { get; internal set; }

        public StartPage()
        {
            InitializeComponent();
            UI = this;

            this.Loaded += StartPage_Loaded;
        }

        private void StartPage_Loaded(object sender, RoutedEventArgs e)
        {
            Info_Music.Text = $"共 {App.DireInfo.Musics?.Count} 首音乐";
            if (App.DireInfo.Musics.Count == 0)
            {
                Info_Music.Text = $"没有音乐 ;D";
                Info_Music.Foreground = new SolidColorBrush(Colors.White);
                RECT_Music.Fill = new SolidColorBrush(Colors.Gray);
                AlumbBorder.IsEnabled = false;
            }
            Info_Vedio.Text = $"共 {App.DireInfo.Videos.Count} 个视频";
            if (App.DireInfo.Videos.Count == 0)
            {
                Info_Vedio.Text = $"没有视频 ;D";
                Info_Vedio.Foreground = new SolidColorBrush(Colors.White);
                RECT_Vedio.Fill = new SolidColorBrush(Colors.Gray);
                VedioBorder.IsEnabled = false;
            }
            Info_Pictrue.Text = $"共 {App.DireInfo.Images.Count} 个图片";
            if (App.DireInfo.Images.Count == 0)
            {
                Info_Pictrue.Text = $"没有图片 ;D";
                Info_Pictrue.Foreground = new SolidColorBrush(Colors.White);
                RECT_Pic.Fill = new SolidColorBrush(Colors.Gray);
                PictruesBorder.IsEnabled = false;
            }
        }

        private void AlumbBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var mw = App.Current.MainWindow as MainWindow;
            mw.mainBorder.Child = SelectAlumbPage.UI;
        }

        private void VedioBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {

            var mw = App.Current.MainWindow as MainWindow;
            mw.mainBorder.Child = SelectVideoPage.UI;
        }

        private void PictruesBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {

            var mw = App.Current.MainWindow as MainWindow;
            mw.mainBorder.Child = PictrueWrap.UI;
        }
    }
}
