using Dtimu.Core;
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
using TagLib.Ape;

namespace DiscViewer.Views
{
    /// <summary>
    /// VideoViewer.xaml 的交互逻辑
    /// </summary>
    public partial class VideoViewer : UserControl
    {
        public static VideoViewer UI = new VideoViewer();
        public VideoViewer()
        {
            InitializeComponent();
            //App.Current.MainWindow.SizeChanged += MainWindow_SizeChanged;
            
        }


        public static void Show(Video info)
        {
            var mw = App.Current.MainWindow as MainWindow;
            UI.Load(info);
            mw.mainBorder.Child = null;
            mw.mainBorder.Child = UI;
        }

        private void Load(Video info)
        {
            var mp4 = System.IO.Path.Combine(App.TargetDire, info.FilePath);
            PlayingBar.PlayVedio(mp4);
            PlayingBar.Show();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            var mw = App.Current.MainWindow as MainWindow;
            mw.mainBorder.Child = null;
            mw.mainBorder.Child = SelectVideoPage.UI;
        }
    }
}
