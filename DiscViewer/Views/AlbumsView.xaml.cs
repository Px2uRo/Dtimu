using DiscViewer.Views.FlyoputView;
using Dtimu.IndexSchemas;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TagLib.Ape;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Windows.Controls.Image;

namespace DiscViewer.Views
{
    class TextColorPair
    {
        public string Text { get; set; }
        public Color Color { get; set; }
        public TextColorPair(string text,Color color)
        {
            Text = text;
            Color = color;
        }
    }
    /// <summary>
    /// AlbumsView.xaml 的交互逻辑
    /// </summary>
    public partial class AlbumsView : UserControl
    {
        public static double SizeOfImage { get; private set; } = 120;
        public int Rows { get; private set; }
        public int Columns { get; private set; }

        public List<object> bits = new List<object>();

        public AlbumsView()
        {
            InitializeComponent();
            this.Loaded += AlbumsView_Loaded;
            App.Current.MainWindow.SizeChanged += MainWindow_SizeChanged;
            LoadAlbums();
        }

        private void AlbumsView_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow_SizeChanged(sender, null);
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double h = 0d;
            if (e == null)
            {
                h = App.Current.MainWindow.Height;
            }
            else
            {
                h = e.NewSize.Height;
            }
            Rows = (int)((h - SizeOfImage * 1) / SizeOfImage);
            Columns = (int)Math.Ceiling(((double)uniformGrid.Children.Count / (Rows)));

            uniformGrid.Columns = Columns;
            uniformGrid.Rows = Rows;
            SC.Height = Rows * SizeOfImage;
            uniformGrid.Height = Rows * SizeOfImage;
            Height = Rows * SizeOfImage + 100;
            MouseOverCanvas.Height = Height;
        }

        private void LoadAlbums()
        {
            
                direInfo = App.DireInfo;

                foreach (var album in direInfo.Albums)
                {
                    var value = album.Value;
                    var firstPicture = value.Pictures.Count > 0 ? value.Pictures[0] : null;
                    if (!string.IsNullOrEmpty(firstPicture))
                    {

                        var b64 = direInfo.Pictures[firstPicture];
                        var picture = ConvertBase64ToBitmapImage(b64);
                        bits.Add(picture);
                        uniformGrid.Children.Add(new Image() { Width = SizeOfImage, Height = SizeOfImage, Source = picture });
                    }
                    else
                    {
                        var c = GenColor();
                        bits.Add(new TextColorPair(album.Key, c));
                        uniformGrid.Children.Add(GenText(album.Key,c));
                    }
                }
            
        }

        private BitmapImage ConvertBase64ToBitmapImage(string base64String)
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
            GC.SuppressFinalize(imageBytes);
            GC.Collect();

            return bitmapImage;
        }

        private Image displayedImage = null;
        int lastimageIndex = 0;
        private int oI;
        private double lstX;
        private double lstY;
        private Border displayedBorder = null;

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            Point mp = Mouse.GetPosition(uniformGrid);
            Point mw = Mouse.GetPosition(App.Current.MainWindow);
            if (Math.Abs(lstX - mw.X) <= 2)
            {
                lstX = mw.X; return;
            }
            else if (Math.Abs(lstY - mw.Y) <= 2)
            {
                lstX = mw.Y; return;
            }
            lstX = mw.X;
            lstY = mw.Y;

            var rw = (int)(mp.Y / SizeOfImage);
            var cl = (int)((mp.X) / SizeOfImage);
            var nI = (rw * uniformGrid.Columns + cl);
            if (nI >= bits.Count) return;
            var newImageSource = bits[nI];
            oI = nI;

            // 如果 displayedBorder 是空的，创建它并添加到画布中
            if (displayedBorder == null)
            {
                displayedBorder = new Border()
                {
                    BorderThickness = new Thickness(5),
                    Width = 200,
                    Height = 200
                };
                displayedBorder.MouseDown += DisplayedBorder_MouseDown;

                displayedImage = new Image() { Width = 200, Height = 200 };
                displayedBorder.Child = displayedImage;

                MouseOverCanvas.Children.Add(displayedBorder);

                // 添加阴影效果到 Border 而不是 Image
                DropShadowEffect shadowEffect = new DropShadowEffect
                {
                    Color = Colors.Black,       // 设置阴影颜色
                    BlurRadius = 10,            // 设置模糊半径
                    ShadowDepth = 5,            // 设置阴影深度
                    Direction = 315,            // 设置阴影方向 (315度, 即右下角)
                    Opacity = 0.5               // 设置阴影透明度
                };
                displayedBorder.Effect = shadowEffect;
            }

            // 更新 Image 的 Source
            if (newImageSource is BitmapImage bi)
            {
                displayedBorder.Child = null;
                displayedImage.Source = bi;
                displayedBorder.Child = displayedImage;
            }
            else if (newImageSource is TextColorPair t)
            {
                ZoomTextAlbum(t);
            }
            else
            {
                return;
            }

            // 更新 Border 的位置
            displayedBorder.SetValue(Canvas.LeftProperty, (cl - SC.HorizontalOffset / SizeOfImage) * SizeOfImage - 25);
            displayedBorder.SetValue(Canvas.TopProperty, rw * SizeOfImage);

            // 动画效果
            if (nI != lastimageIndex)
            {
                ReRunAnimAndViewInfos(nI);
            }

            lastimageIndex = nI;
        }

        private void ZoomTextAlbum(TextColorPair t)
        {
            displayedBorder.Child = null;
            s.Color = t.Color;
            displayedG.Background = s;
            displayedG.Children.Clear();
            displayedText.Text = t.Text;
            displayedG.Children.Add(displayedText);
            displayedBorder.Child = displayedG;
        }

        private void DisplayedBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var mw = App.Current.MainWindow as MainWindow;
            mw.flyo.Child=( AlumbInfo.Current);
            AlumbInfo.Current.Show();
            if (bits[_nI] is BitmapImage bi)
            {
                AlumbInfo.Current.LoadAlumb(direInfo.Albums.Values.ToArray()[_nI], direInfo.Albums.Keys.ToArray()[_nI],bi);
            }
            else
            {
                AlumbInfo.Current.LoadAlumb(direInfo.Albums.Values.ToArray()[_nI], direInfo.Albums.Keys.ToArray()[_nI],null);

            }
        }

        private void ReRunAnimAndViewInfos(int nI)
        {
            _nI = nI;
            var alumb = direInfo.Albums.ToList()[nI];
            TitleBox.Text = alumb.Key;
            PerformancerBox.Text = string.Join("/",alumb.Value.AlbumPerformers.ToArray());
            ScaleTransform scale = new ScaleTransform(1.0, 1.0);
            displayedBorder.RenderTransform = scale;
            displayedBorder.RenderTransformOrigin = new Point(0.5, 0.5);  // 设置中心为控件的中点

            var easing = new ExponentialEase()
            {
                EasingMode = EasingMode.EaseOut
            };

            DoubleAnimation zoomInX = new DoubleAnimation(1.0, 1.02, TimeSpan.FromSeconds(0.2))
            {
                EasingFunction = easing
            };
            DoubleAnimation zoomInY = new DoubleAnimation(1.0, 1.02, TimeSpan.FromSeconds(0.2))
            {
                EasingFunction = easing
            };

            // 启动动画
            scale.BeginAnimation(ScaleTransform.ScaleXProperty, zoomInX);
            scale.BeginAnimation(ScaleTransform.ScaleYProperty, zoomInY);
        }
        TextBlock displayedText = new TextBlock()
        {
            Foreground = new SolidColorBrush(Colors.White),
                FontSize = 20,
                Width = SizeOfImage - 10,
                Height = SizeOfImage - 10,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

        Grid displayedG = new Grid();
        SolidColorBrush s = new SolidColorBrush();
        private void UserControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (oI + 1 == bits.Count)
            {
                if (e.Delta < 0)
                {
                    ReRunAnimAndViewInfos(bits.Count-1);
                    return;
                }
            }
            else if (oI == 0)
            {
                if (e.Delta > 0)
                {
                    ReRunAnimAndViewInfos(0);
                    return;
                }
            }
            var upLeft = true;
            var LeftSet = Math.Floor(SC.HorizontalOffset / SizeOfImage);
            if (e.Delta > 0)
            {
                oI -= 1;
                if (((oI + 1) % Columns) * SizeOfImage - SC.HorizontalOffset <= 0)
                {
                    SC.ScrollToHorizontalOffset(SizeOfImage * (oI % Columns));
                    upLeft = false;
                }

            }
            else if (e.Delta < 0)
            {
                oI += 1;
                if (((oI + 1) % Columns) * SizeOfImage - SC.HorizontalOffset >= ActualWidth)
                {
                    SC.ScrollToHorizontalOffset(SC.HorizontalOffset + SizeOfImage);
                    upLeft = false;
                }
            }
            if (oI % Columns <= 1)
            {
                SC.ScrollToHorizontalOffset(0);
                displayedBorder.SetValue(Canvas.TopProperty, SizeOfImage * Math.Floor((double)oI / Columns));
            }
            else if (Columns - oI % Columns <= 1)
            {
                SC.ScrollToRightEnd();
                displayedBorder.SetValue(Canvas.TopProperty, SizeOfImage * Math.Floor((double)oI / Columns));
                upLeft = true;
            }
            var newImageSource = bits[oI];

            if (newImageSource is BitmapImage bi)
            {
                displayedBorder.Child = null;
                displayedImage.Source = bi;
                displayedBorder.Child = displayedImage;
            }
            else if (newImageSource is TextColorPair t)
            {
                ZoomTextAlbum(t);
            }
            else
            {
                return;
            }
            if (upLeft)
            {
                displayedBorder.SetValue(Canvas.LeftProperty, ((oI % Columns) - LeftSet) * SizeOfImage - 25 - SC.HorizontalOffset % SizeOfImage);
            }

            ReRunAnimAndViewInfos(oI);
            e.Handled = true;
        }
        private Random random = new Random();
        private int lstRand = -1;
        private Color[] allColors = new Color[]
{
    Colors.Red,         // 赤
    Colors.Orange,      // 橙
    Colors.Green,       // 绿
    Colors.Blue,        // 蓝
    Colors.Purple,      // 紫
    Colors.DarkRed,     // 深红
    Colors.DarkOrange,  // 深橙
    Colors.DarkGreen,   // 深绿
    Colors.DarkBlue,    // 深蓝
    Colors.Brown,       // 棕色
    Colors.DarkViolet,  // 深紫
    Colors.Teal,        // 水鸭色
    Colors.Maroon,      // 栗色
    Colors.Navy,        // 海军蓝
    Colors.Indigo,      // 靛蓝
    Colors.DarkCyan,    // 深青
    Colors.DarkMagenta  // 深洋红
};
        private DireInfo direInfo;
        private int _nI;

        private void ShuffleColors()
        {
            for (int i = allColors.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                var temp = allColors[i];
                allColors[i] = allColors[j];
                allColors[j] = temp;
            }
        }
        private Grid GenText(string t,Color c)
        {
            var res = new Grid() { Background = new SolidColorBrush(c) };
            var te = new TextBlock()
            {
                Foreground = new SolidColorBrush(Colors.White),
                FontSize = 20,
                Width = SizeOfImage - 10,
                Height = SizeOfImage - 10,
                Text = t.ToUpper(),
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            res.Children.Add(te);
            return res;

        }

        private Color GenColor()
        {
            ShuffleColors();

            // 从洗牌后的颜色中选择一个与上次不同的颜色
            int newIndex;
            do
            {
                newIndex = random.Next(allColors.Length);
            } while (newIndex == lstRand); // 避免连续相同颜色

            lstRand = newIndex; // 记录当前使用的颜色索引

            var randomColor = allColors[newIndex];
            return randomColor;
        }

        private void PlayAll_MouseDown(object sender, MouseButtonEventArgs e)
        {
            foreach (var item in this.direInfo.Musics)
            {
                Models.Lists.Playing.Add(System.IO.Path.Combine(App.TargetDire,item.Value.FileName));
            }
            PlayingBar.PlayMusic(System.IO.Path.Combine(App.TargetDire, direInfo.Musics.Values.First().FileName));
            PlayingBar.Show();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            var mw = App.Current.MainWindow as MainWindow;
            mw.mainBorder.Child = null;
            mw.mainBorder.Child = StartPage.UI;
        }
    }
}
