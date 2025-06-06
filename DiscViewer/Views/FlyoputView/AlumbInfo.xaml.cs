using Dtimu.IndexSchemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace DiscViewer.Views.FlyoputView
{
    /// <summary>
    /// AlumbInfo.xaml 的交互逻辑
    /// </summary>
    public partial class AlumbInfo : UserControl
    {
        public static AlumbInfo Current { get; set; } = new AlumbInfo();
        public AlumbInfo()
        {
            InitializeComponent();

            DropShadowEffect shadowEffect = new DropShadowEffect
            {
                Color = Colors.Black,       // 设置阴影颜色
                BlurRadius = 10,            // 设置模糊半径
                ShadowDepth = 5,            // 设置阴影深度
                Direction = 315,            // 设置阴影方向 (315度, 即右下角)
                Opacity = 0.5               // 设置阴影透明度
            };
            this.Effect = shadowEffect;
        }
        internal void LoadAlumb(Album album,string name,BitmapImage bi)
        {
            _album = album;
            Musics.Children.Clear();
            AlbumImage.Source = bi;
            if (bi == null&& anys.Children.Contains(remvb))
            {
                anys.Children.Remove(remvb);
            }
            else if(bi != null && anys.Children.Contains(remvb) == false)
            {
                anys.Children.Insert(0,remvb);
            }
            TitleBox.Text = name;
            PerformancerBox.Text = string.Join("/", album.AlbumPerformers.ToArray());

            foreach (var item in album.Musics)
            {
                var ele = new Grid();
                ele.Height = 25;
                ele.Background = new SolidColorBrush(Colors.Transparent);
                ele.DataContext = item;
                ele.MouseDown += Ele_MouseDown;
                ele.MouseEnter += Ele_MouseEnter;
                ele.MouseLeave += Ele_MouseLeave;
                ele.Children.Add(new TextBlock() {Margin=new Thickness(20,0,0,0),VerticalAlignment=VerticalAlignment.Center
                    , Text = App.DireInfo.Musics[item].Title });
                Musics.Children.Add(ele);
            }
        }

        private void Ele_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var mw = App.Current.MainWindow as MainWindow;
            var ms = (sender as Grid).DataContext as string;
            var p = FindFile(ms);
            PlayingBar.Show();
            PlayingBar.PlayMusic(p);
            mw.flyo.Child = null;
            //mw.Content = MusicViewer.Current;
        }

        public static string FindFile(string hash)
        {
            var baseD = App.TargetDire;
            return System.IO.Path.Combine(baseD,App.DireInfo.Musics[hash].FileName);
        }

        private void Ele_MouseLeave(object sender, MouseEventArgs e)
        {
            var grd = sender as Grid;
            grd.Background.BeginAnimation(SolidColorBrush.ColorProperty, carvs );
        }

        private void Ele_MouseEnter(object sender, MouseEventArgs e)
        {
            var grd = sender as Grid;
            grd.Background.BeginAnimation(SolidColorBrush.ColorProperty, ca);
        }

        internal void Show()
        {
            this.Visibility = Visibility.Visible;

            if (!PlayingBar.Current.Stopped)
            {
                this.BeginAnimation(MarginProperty, MarginAnim(this.DesiredSize.Height,50));
            }
            else
            {
                this.BeginAnimation(MarginProperty, MarginAnim(this.DesiredSize.Height,0));
            }
        }
        static ThicknessAnimation MarginAnim(double height,double finalH) => new ThicknessAnimation()
        {
            From = new Thickness(30, 0, 30, 0-height),
            To = new Thickness(30, 0, 30, finalH),
            Duration = new Duration(TimeSpan.FromSeconds((finalH-(0-height))/1600)),
            EasingFunction = ease
        };
        static IEasingFunction ease = new PowerEase();

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
            To = Colors.White ,
            Duration = TimeSpan.FromSeconds(0.2),
            AutoReverse = false,
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut } // 缓动函数
        };
        private Album _album;
    }
}
