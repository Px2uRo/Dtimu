using Dtimu.IndexSchemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TagLib.Ape;

namespace DiscViewer.Views.FlyoputView
{
    /// <summary>
    /// PlayList.xaml 的交互逻辑
    /// </summary>
    public partial class PlayList : UserControl
    {
        public static PlayList Current { get; set; } = new PlayList();
        public PlayList()
        {
            InitializeComponent();
            foreach (var item in Models.Lists.Playing)
            {
                AddMusic(item);
            }
            Models.Lists.Playing.CollectionChanged += Playing_CollectionChanged;
            SC.Height = App.Current.MainWindow.Height - 60;
            App.Current.MainWindow.SizeChanged += MainWindow_SizeChanged;
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SC.Height = e.NewSize.Height-60;
        }

        private void Playing_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    AddMusic(item.ToString());
                }
            }
        }

        internal void Show()
        {
            var mw = App.Current.MainWindow as MainWindow;
            mw.flyo.Child = null;
            mw.flyo.Child = this;
            PlayAnim();
        }

        private void PlayAnim()
        {

        }

        internal void AddMusic(string item)
        {
            var ele = new Grid();
            ele.Height = 25;
            ele.Background = new SolidColorBrush(Colors.Transparent);
            ele.DataContext = item;
            ele.MouseDown += Ele_MouseDown;
            ele.MouseEnter += Ele_MouseEnter;
            ele.MouseLeave += Ele_MouseLeave;
            ele.Children.Add(new TextBlock()
            {
                Margin = new Thickness(20, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                Text = System.IO.Path.GetFileName(item)
            }) ;
            MyGrid.Children.Add(ele);
        }

        ColorAnimation ca = new ColorAnimation
        {
            From = Colors.White,
            To = Colors.LightGray,
            Duration = TimeSpan.FromSeconds(0.2),
            AutoReverse = false,
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut } // 缓动函数
        };

        ColorAnimation carvs = new ColorAnimation
        {
            From = Colors.LightGray,
            To = Colors.White,
            Duration = TimeSpan.FromSeconds(0.2),
            AutoReverse = false,
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut } // 缓动函数
        };
        private void Ele_MouseLeave(object sender, MouseEventArgs e)
        {
            var grd = sender as Grid;
            grd.Background.BeginAnimation(SolidColorBrush.ColorProperty, carvs);
        }

        private void Ele_MouseEnter(object sender, MouseEventArgs e)
        {
            var grd = sender as Grid;
            grd.Background.BeginAnimation(SolidColorBrush.ColorProperty, ca);
        }

        private void Ele_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var mw = App.Current.MainWindow as MainWindow;
            var ms = (sender as Grid).DataContext as string;
            if (PlayingBar.Current.Margin.Bottom != 0)
            {
                PlayingBar.Show();
            }

            if (ms.EndsWith(".mp4"))
            {
                PlayingBar.PlayVedio(ms);
            }
            else
                {
                PlayingBar.PlayMusic(ms);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var mw = App.Current.MainWindow as MainWindow;
            mw.flyo.Child = null;
        }
    }
}
