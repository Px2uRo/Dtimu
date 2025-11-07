#region Using 指令
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
#endregion

namespace DiscViewer.Views
{
    #region 辅助类
    class TextColorPair
    {
        public string Text { get; set; }
        public Color Color { get; set; }
        public TextColorPair(string text, Color color)
        {
            Text = text;
            Color = color;
        }
    }
    #endregion

    /// <summary>
    /// AlbumsView.xaml 的交互逻辑
    /// </summary>
    public partial class AlbumsView : UserControl
    {
        #region 字段与属性

        public static double SizeOfImage { get; private set; } = 120;
        public int Rows { get; private set; }
        public int Columns { get; private set; }

        public List<object> bits = new List<object>();
        private DireInfo direInfo;
        private int _nI;
        private bool search_expand = false;

        private Image displayedImage = null;
        int lastimageIndex = 0;
        private int oI;
        private double lstX;
        private double lstY;
        private Border displayedBorder = null;

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

        private Random random = new Random();
        private int lstRand = -1;
        private Color[] allColors = new Color[]
        {
            Colors.Red, Colors.Orange, Colors.Green, Colors.Blue, Colors.Purple,
            Colors.DarkRed, Colors.DarkOrange, Colors.DarkGreen, Colors.DarkBlue,
            Colors.Brown, Colors.DarkViolet, Colors.Teal, Colors.Maroon,
            Colors.Navy, Colors.Indigo, Colors.DarkCyan, Colors.DarkMagenta
        };

        #endregion

        #region 构造函数与初始化

        public AlbumsView()
        {
            InitializeComponent();
            this.Loaded += AlbumsView_Loaded;
            App.Current.MainWindow.SizeChanged += MainWindow_SizeChanged;
            direInfo = App.DireInfo;
            LoadAlbums();
        }

        private void AlbumsView_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow_SizeChanged(sender, null);
        }

        #endregion

        #region 布局处理

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double h = e?.NewSize.Height ?? App.Current.MainWindow.Height;
            double w = e?.NewSize.Width ?? App.Current.MainWindow.Width;

            Rows = (int)((h - SizeOfImage * 1) / SizeOfImage);
            Columns = (int)Math.Ceiling(((double)uniformGrid.Children.Count / Rows));

            uniformGrid.Columns = Columns;
            uniformGrid.Rows = Rows;
            SC.Height = Rows * SizeOfImage;
            uniformGrid.Height = Rows * SizeOfImage;
            Height = Rows * SizeOfImage + 100;
            MouseOverCanvas.Height = Height;

            if (search_expand)
            {
                AnimateWidth(SearchBtn, w - 100, 0.3);
                AnimateWidth(SearchBox, w - 120, 0.3);
            }
        }

        #endregion

        #region 数据加载与显示
        private void LoadAlbums()
        {
            LoadAlbums(direInfo.Albums);
        }
        private void LoadAlbums(Dictionary<string, Album> albums)
        {
            foreach (var album in albums)
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
                    uniformGrid.Children.Add(GenText(album.Key, c));
                }
            }
        }

        private BitmapImage ConvertBase64ToBitmapImage(string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
                return null;

            byte[] imageBytes = Convert.FromBase64String(base64String);
            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = ms;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }
            GC.SuppressFinalize(imageBytes);
            GC.Collect();
            return bitmapImage;
        }

        #endregion

        #region 鼠标事件

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            Point mp = Mouse.GetPosition(uniformGrid);
            Point mw = Mouse.GetPosition(App.Current.MainWindow);
            if (Math.Abs(lstX - mw.X) <= 2 && Math.Abs(lstY - mw.Y) <= 2)
                return;

            lstX = mw.X;
            lstY = mw.Y;

            var rw = (int)(mp.Y / SizeOfImage);
            var cl = (int)(mp.X / SizeOfImage);
            var nI = (rw * Columns + cl);
            if (nI >= bits.Count) return;
            oI = nI;
            var newImageSource = bits[nI];

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

                DropShadowEffect shadowEffect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = 10,
                    ShadowDepth = 5,
                    Direction = 315,
                    Opacity = 0.5
                };
                displayedBorder.Effect = shadowEffect;

                MouseOverCanvas.Children.Add(displayedBorder);
            }

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

            displayedBorder.SetValue(Canvas.LeftProperty, (cl - SC.HorizontalOffset / SizeOfImage) * SizeOfImage - 25);
            displayedBorder.SetValue(Canvas.TopProperty, rw * SizeOfImage);

            if (nI != lastimageIndex)
                ReRunAnimAndViewInfos(nI);

            lastimageIndex = nI;
        }

        private void UserControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((oI + 1 == bits.Count && e.Delta < 0) || (oI == 0 && e.Delta > 0))
            {
                ReRunAnimAndViewInfos(oI);
                return;
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
            else
            {
                oI += 1;
                if (((oI + 1) % Columns) * SizeOfImage - SC.HorizontalOffset >= ActualWidth)
                {
                    SC.ScrollToHorizontalOffset(SC.HorizontalOffset + SizeOfImage);
                    upLeft = false;
                }
            }

            if (oI % Columns <= 1)
                SC.ScrollToHorizontalOffset(0);
            else if (Columns - oI % Columns <= 1)
                SC.ScrollToRightEnd();

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

            if (upLeft)
            {
                displayedBorder.SetValue(Canvas.LeftProperty, ((oI % Columns) - LeftSet) * SizeOfImage - 25 - SC.HorizontalOffset % SizeOfImage);
            }

            ReRunAnimAndViewInfos(oI);
            e.Handled = true;
        }

        private void DisplayedBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var mw = App.Current.MainWindow as MainWindow;
            mw.flyo.Child = AlumbInfo.Current;
            AlumbInfo.Current.Show();

            if (bits[_nI] is BitmapImage bi)
                AlumbInfo.Current.LoadAlumb(direInfo.Albums.Values.ToArray()[_nI], direInfo.Albums.Keys.ToArray()[_nI], bi);
            else
                AlumbInfo.Current.LoadAlumb(direInfo.Albums.Values.ToArray()[_nI], direInfo.Albums.Keys.ToArray()[_nI], null);
        }

        #endregion

        #region 视觉动画与渲染

        private void ReRunAnimAndViewInfos(int nI)
        {
            _nI = nI;
            var alumb = direInfo.Albums.ToList()[nI];
            TitleBox.Text = alumb.Key;
            PerformancerBox.Text = string.Join("/", alumb.Value.AlbumPerformers.ToArray());

            ScaleTransform scale = new ScaleTransform(1.0, 1.0);
            displayedBorder.RenderTransform = scale;
            displayedBorder.RenderTransformOrigin = new Point(0.5, 0.5);

            var easing = new ExponentialEase() { EasingMode = EasingMode.EaseOut };
            DoubleAnimation zoomInX = new DoubleAnimation(1.0, 1.02, TimeSpan.FromSeconds(0.2)) { EasingFunction = easing };
            DoubleAnimation zoomInY = new DoubleAnimation(1.0, 1.02, TimeSpan.FromSeconds(0.2)) { EasingFunction = easing };

            scale.BeginAnimation(ScaleTransform.ScaleXProperty, zoomInX);
            scale.BeginAnimation(ScaleTransform.ScaleYProperty, zoomInY);
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

        #endregion

        #region UI 生成与颜色

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

        private Grid GenText(string t, Color c)
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
            int newIndex;
            do
            {
                newIndex = random.Next(allColors.Length);
            } while (newIndex == lstRand);
            lstRand = newIndex;
            return allColors[newIndex];
        }

        #endregion

        #region 控件交互事件

        private void PlayAll_MouseDown(object sender, MouseButtonEventArgs e)
        {
            foreach (var item in this.direInfo.Musics)
            {
                Models.Lists.Playing.Add(System.IO.Path.Combine(App.TargetDire, item.Value.FileName));
            }
            PlayingBar.PlayMusic(System.IO.Path.Combine(App.TargetDire, direInfo.Musics.Values.First().FileName));
            PlayingBar.Show();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            var mw = App.Current.MainWindow as MainWindow;
            mw.mainBorder.Child = StartPage.UI;
        }

        private void SearchBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!search_expand)
            {
                search_expand = true;
                AnimateWidth(sender as FrameworkElement, App.Current.MainWindow.Width - 100, 0.3);
                SearchBtn.ButtonText = string.Empty;
                SearchBox.Visibility = Visibility.Visible;
                AnimateWidth(SearchBox, App.Current.MainWindow.Width - 120, 0.3);
            }
        }

        private void SearchBtn_KeyDown(object sender, KeyEventArgs e)
        {
            // 空方法预留
        }

        #endregion

        #region 辅助动画方法

        private void AnimateWidth(FrameworkElement element, double targetWidth, double durationSeconds = 0.3)
        {
            if (!element.IsLoaded)
            {
                element.Loaded += (s, e) => AnimateWidth(element, targetWidth, durationSeconds);
                return;
            }

            double fromWidth = element.ActualWidth;
            if (double.IsNaN(fromWidth) || fromWidth == 0)
                fromWidth = element.RenderSize.Width;

            var animation = new DoubleAnimation
            {
                From = fromWidth,
                To = targetWidth,
                Duration = TimeSpan.FromSeconds(durationSeconds),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };
            element.BeginAnimation(FrameworkElement.WidthProperty, animation);
        }

        #endregion
    }
}
