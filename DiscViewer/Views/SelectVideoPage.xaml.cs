using DiscViewer.SmallControls;
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
    /// SelectVideoPage.xaml 的交互逻辑
    /// </summary>
    public partial class SelectVideoPage : UserControl
    {
        public static UIElement UI { get; internal set; } = new SelectVideoPage();

        public SelectVideoPage()
        {
            this.Loaded += SelectVideoPage_Loaded;
            InitializeComponent();
        }

        private void SelectVideoPage_Loaded(object sender, RoutedEventArgs e)
        {
            App.Current.MainWindow.SizeChanged -= Window_SizeChanged;
            App.Current.MainWindow.SizeChanged += Window_SizeChanged;
            Window_SizeChanged(this,null);
            LoadVideos();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Wraps.Height = double.NaN;
            // 获取窗口的实际宽高
            double width = ActualWidth;
            if(sender is Window w&&w.WindowState==WindowState.Maximized) 
            {
                width = SystemParameters.FullPrimaryScreenWidth;
            }

            // 设置每个单元格的最小宽度和高度
            const double minCellWidth = 300;

            // 计算列数和行数
            int columns = Math.Max(1, (int)(width / minCellWidth));
            int rows = Math.Max(1, (int)(App.DireInfo.Videos.Count / columns));
            if(rows * 100 < ActualHeight)
            {
                Wraps.Height = rows * 100;
            }
            // 更新 UniformGrid 的列数和行数
            Wraps.Columns = columns;
            Wraps.Rows = rows;
        }

        private void LoadVideos()
        {
            Wraps.Children.Clear();
            foreach (var item in App.DireInfo.Videos)
            {
                var child = new VedioDetail();
                child.DataContext = item.Value;
                Wraps.Children.Add(child);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {

            var mw = App.Current.MainWindow as MainWindow;
            mw.mainBorder.Child = null;
            mw.mainBorder.Child = StartPage.UI;
        }
    }
}
