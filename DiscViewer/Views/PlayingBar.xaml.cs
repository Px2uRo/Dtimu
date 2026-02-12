using DiscViewer.Views.FlyoputView;
using Dtimu;
using Dtimu.IndexSchemas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TagLib.Ape;
using TagLib.Matroska;
using TagLib.Riff;

namespace DiscViewer.Views
{
    /// <summary>
    /// PlayingBar.xaml 的交互逻辑
    /// </summary>
    public partial class PlayingBar : UserControl
    {
        internal MediaElement _media1 = new MediaElement();
        private string _currentFile;
        public TagLib.Tag _tag;

        public static PlayingBar Current { get; set; }
        public bool Stopped { get => _isStopped; }
        public CompetedAction ca { get; set; } = CompetedAction.RepeatList;
        public bool _hidden = false;
        public bool Hidden
        {
            get => _hidden;
            set
            {
                _hidden = value;
                Current.HiddenPropChanged?.Invoke(Current, new());
            }
        }

        public bool EnableAnimAndEvent { get; private set; } = true;

        public event EventHandler<EventArgs> HiddenPropChanged;
        private Thumb FindThumb(FrameworkElement slider)
        {
            // 遍历视觉树以查找Thumb控件
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(slider); i++)
            {
                var child = VisualTreeHelper.GetChild(slider, i) as FrameworkElement;
                if (child is Thumb)
                {
                    return child as Thumb;
                }
                // 递归查找子元素
                var result = FindThumb(child);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
        Thumb _thumb;
        public PlayingBar()
        {
            InitializeComponent();
            Current = this;
            SeekToBar.Loaded += SeekToBar_Loaded;


            VolT.Text = "50";
            //MouseEnter += UserControl_MouseEnter;
            //MouseLeave += UserControl_MouseLeave;
            MarginAnim.Completed += MarginAnim_Completed;
            MarginAnim2.Completed += MarginAnim2_Completed;
            VideoViewer.UI.media.MediaEnded += _media1_MediaEnded;
            AlumbInfo.Current.IsVisibleChanged += Current_IsVisibleChanged;
        }

        private void Current_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(sender as AlumbInfo is AlumbInfo alumb)
            {
                if(App.Current.MainWindow is MainWindow window)
                {
                    if (alumb.IsVisible)
                    {
                        this.EnableAnimAndEvent = false;
                    }
                    else
                    {
                        this.EnableAnimAndEvent = true;
                    }
                }
            }
        }

        private void _media1_MediaEnded(object sender, RoutedEventArgs e)
        {
            var m = sender as MediaElement;
            if(ca == CompetedAction.RepeatOne)
            {
                if (_playingVedio)
                {
                    playVedio(_currentFile);
                }
                else
                {
                    playMusic(_currentFile);
                }
            }
            else if (ca == CompetedAction.RepeatList)
            {
                var ined = (Models.Lists.Playing.IndexOf(_currentFile) +1) % Models.Lists.Playing.Count;
                _currentFile = Models.Lists.Playing[ined];
                if (_currentFile.EndsWith(".mp4"))
                {
                    PlayVedio(_currentFile);
                }
                else 
                {
                    PlayMusic(_currentFile);
                }
            }
            else if (ca == CompetedAction.Randomly)
            {
                var rand = new Random();
                var ined = rand.Next(0, Models.Lists.Playing.Count) % Models.Lists.Playing.Count;
                while (ined == Models.Lists.Playing.IndexOf(_currentFile)&& Models.Lists.Playing.Count>1)
                {
                    ined = rand.Next(0, Models.Lists.Playing.Count) % Models.Lists.Playing.Count;
                }
                _currentFile = Models.Lists.Playing[ined];
                if (_currentFile.EndsWith(".mp4"))
                {
                    PlayVedio(_currentFile);
                }
                else 
                {
                    PlayMusic(_currentFile);
                }
            }
        }

        private void MarginAnim2_Completed(object sender, EventArgs e)
        {
        //    Current.MouseEnter += UserControl_MouseEnter;
        }

        private void MarginAnim_Completed(object sender, EventArgs e)
        {
        //    Current.MouseLeave += UserControl_MouseLeave;
        }

        private void SeekToBar_Loaded(object sender, RoutedEventArgs e)
        {
            _thumb = FindThumb(SeekToBar);

            // 绑定 Thumb 的事件
            if (_thumb != null)
            {
                _thumb.DragDelta += _thumb_DragDelta;
                _thumb.DragStarted += (s, e2) =>
                {
                    isSliderBeingDragged = true; // 开始拖动
                };

                _thumb.DragCompleted += (s, e2) =>
                {
                    isSliderBeingDragged = false; // 完成拖动
                                                  // 更新 waveStream 的 CurrentTime
                    if (_playingVedio)
                    {

                        if (VideoViewer.UI.media != null)
                        {
                            VideoViewer.UI.media.Position = TimeSpan.FromSeconds(SeekToBar.Value);
                        }
                    }
                    else
                    {
                        if (_media1 != null)
                        {
                            _media1.Position = TimeSpan.FromSeconds(SeekToBar.Value);
                        }
                    }
                };

            }
        }

        private void _thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var sec = Mouse.GetPosition(SeekToBar).X / SeekToBar.ActualWidth * SeekToBar.Maximum;
            var x = Mouse.GetPosition(SeekToBar).X;
            if (x < 0)
            {
                sec = 0;
            }
            else if (x > SeekToBar.ActualWidth)
            {
                sec = SeekToBar.Maximum;
            }
            PositionInfo.Text = $"{TimeSpan.FromSeconds(sec).ToString(@"mm\:ss")} / {TimeSpan.FromSeconds(SeekToBar.Maximum).ToString(@"mm\:ss")}";
        }

        private void playMusic(string path)
        {
            _media1.LoadedBehavior = MediaState.Manual;
            _media1.UnloadedBehavior = MediaState.Manual;
            _media1.MediaEnded -= _media1_MediaEnded;
            _media1.MediaEnded += _media1_MediaEnded;
            stop();
            _media1.Source = new Uri(path);
            _media1.Volume = (float)(VolS.Value / 1d);
            _currentFile = path;
            using (var file = TagLib.File.Create(_currentFile))
            {
                _tag = file.Tag;
            }
            string artist = _tag.Performers.Length > 0 ? _tag.Performers[0] : "Unknown Artist";
            string album = _tag.Album ?? "Unknown Album";
            string title = _tag.Title ?? "Unknown Title";

            AlbumNameBox.Visibility = Visibility.Visible;
            ASeprector.Visibility = Visibility.Visible;
            PerformersBox.Text = artist;
            AlbumNameBox.Text = "(" + album + ")";
            TitleBox.Text = title;
            FirstLetterBox.Text = title.First().ToString();
            if (AlumbCover.Source != null)
            {
                GC.SuppressFinalize(AlumbCover.Source);
            }
            AlumbCover.Source = ConvertPictrueDataToBitmapImage(_tag.Pictures.FirstOrDefault()?.Data?.ToArray());
            //App.AddRangeToList(new String[] { path });

            UIPlay();
        }
        bool isSliderBeingDragged = false; // 标记是否正在拖动 Slider
        private void UIPlay(bool playingVedio = false)
        {
            SeekToBar.Value = 0;
            this.PlayBtnGroup.Visibility = Visibility.Hidden;
            this.stopBtnGroup.Visibility = Visibility.Visible;
            _playingVedio = playingVedio;

            if (_playingVedio)
            {
                vedioplay();
            }
            else
            {
                musicplay();
            }
            new Thread(() =>
            {
                if (!_playingVedio)
                {
                    if (_media1 == null)
                    {
                        return;
                    }
                    var booled = false;
                    while (!booled)
                    {
                        Thread.Sleep(200);
                        Dispatcher.Invoke(new MyAction(() =>
                        {
                            booled = _media1.NaturalDuration.HasTimeSpan;
                        }));
                    }
                    Dispatcher.Invoke(new MyAction(() =>
                    {
                        SeekToBar.Maximum = _media1.NaturalDuration.TimeSpan.TotalSeconds;
                    }));
                }
                else
                {
                    var booled = false;
                    while (!booled)
                    {
                        Thread.Sleep(100);
                        Dispatcher.Invoke(new MyAction(() =>
                        {
                            booled = VideoViewer.UI.media.NaturalDuration.HasTimeSpan;
                        }));
                    }
                    Dispatcher.Invoke(new MyAction(() =>
                    {
                        SeekToBar.Maximum = VideoViewer.UI.media.NaturalDuration.TimeSpan.TotalSeconds;
                    }));
                }
            }).Start();
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500) // 每500ms更新一次
            };
            timer.Tick += (s, e) =>
            {
                if (!isSliderBeingDragged)
                {
                    if (_uri == null)
                    {
                        return;
                    }
                    if (Mouse.LeftButton == MouseButtonState.Pressed)
                    {
                        return;
                    }
                    if (_playingVedio)
                    {
                        SeekToBar.Value = VideoViewer.UI.media.Position.TotalSeconds;
                        PositionInfo.Text = $"{VideoViewer.UI.media.Position.ToString(@"mm\:ss")} / {TimeSpan.FromSeconds(SeekToBar.Maximum).ToString(@"mm\:ss")}";
                    }
                    else
                    {
                        SeekToBar.Value = _media1.Position.TotalSeconds;
                        PositionInfo.Text = $"{_media1.Position.ToString(@"mm\:ss")} / {TimeSpan.FromSeconds(SeekToBar.Maximum).ToString(@"mm\:ss")}";

                    }
                }
            };
            timer.Start();
        }

        public delegate void MyAction();
        private void vedioplay()
        {
            if (_uri == null || VideoViewer.UI.media.Source.LocalPath != _uri.LocalPath)
            {
                try
                {
                    _uri = new Uri(_currentFile);
                    VideoViewer.UI.media.Source = _uri;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            VideoViewer.UI.media?.Play();
            _isPaused = false;
            _isStopped = false;
            if (!Models.Lists.Playing.Contains(_currentFile))
            {
                Models.Lists.Playing.Add(_currentFile);
            }
            Played?.Invoke(this,new RoutedEventArgs());
        }

        private void musicplay()
        {
            if (_uri == null || _media1.Source.LocalPath != _uri.LocalPath)
            {
                try
                {
                    _uri = new Uri(_currentFile);
                    _media1.Source = _uri;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            _isPaused = false;_isStopped = false;
            Played?.Invoke(this, new RoutedEventArgs());
            _media1?.Play();
        }

        private void pause()
        {
            if (_playingVedio)
            {

                if (VideoViewer.UI.media != null && _isPaused == false)
                {
                    VideoViewer.UI.media.Pause();
                    _isPaused = true;
                }
            }
            else
            {

                if (_media1 != null && _isPaused == false)
                {
                    _media1.Pause();
                    _isPaused = true;
                }
            }
        }

        private void stop()
        {
            if (_playingVedio)
            {

                VideoViewer.UI.media.Stop();
            }
            else
            {
                _media1.Stop();
            }
            _isPaused = true;
            _isStopped = true;
        }

        private ImageSource ConvertPictrueDataToBitmapImage(byte[] bytes)
        {
            if (bytes == null) return null;
            try
            {
                BitmapImage bitmapImage = new BitmapImage();
                using (MemoryStream memoryStream = new MemoryStream(bytes))
                {
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = memoryStream;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze(); // 冻结位图使其可以跨线程访问
                }
                return bitmapImage;

            }
            catch (System.IO.IOException ex)
            {
                return null;
            }
        }

        internal static void PlayMusic(string path)
        {
            Current.playMusic(path);
            if (!Models.Lists.Playing.Contains(path))
            {
                Models.Lists.Playing
                .Add(path);
            }
        }

        internal static void Show()
        {
            if (_notResponsing)
            {
                return;
            }
            new Thread(() =>
            {
                _notResponsing = true;
                Thread.Sleep(200);
                Current.Dispatcher.Invoke(new MyAction(() =>
                {
                    Current.Visibility = Visibility.Visible;
                    //Current.MouseLeave -= UserControl_MouseLeave;
                    if (Current.EnableAnimAndEvent)
                    {
                        Current.Hidden = false;
                        Current.BeginAnimation(MarginProperty, MarginAnim);
                    }
                }));
                _notResponsing = false;
            }).Start();
        }
        static ThicknessAnimation MarginAnim = new ThicknessAnimation()
        {
            From = new Thickness(0, 0, 0, -61),
            To = new Thickness(0, 0, 0, 0),
            Duration = new Duration(TimeSpan.FromSeconds(0.1)),
            EasingFunction = sinease
        };
        static ThicknessAnimation MarginAnim2 = new ThicknessAnimation()
        {
            From = new Thickness(0, 0, 0, 0),
            To = new Thickness(0, 0, 0, -90),
            Duration = new Duration(TimeSpan.FromSeconds(0.1)),
            EasingFunction = sinease
        };
        static ObjectAnimationUsingKeyFrames BooledAnim()
        {
            ObjectAnimationUsingKeyFrames animation = new ObjectAnimationUsingKeyFrames();

            animation.KeyFrames.Add(new DiscreteObjectKeyFrame(Visibility.Visible, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)))); // 0秒时禁用
            animation.KeyFrames.Add(new DiscreteObjectKeyFrame(Visibility.Hidden, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.1))));  // 1秒时启用
            animation.FillBehavior = FillBehavior.Stop;
            return animation;
        }
        static IEasingFunction sinease = new SineEase();
        private Uri _uri;
        internal bool _isPaused;
        internal static bool _playingVedio;
        private static bool _notResponsing;
        internal bool _isStopped = true;
        public event EventHandler<RoutedEventArgs> Played;

        private void SeekToBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_playingVedio)
            {
                if (VideoViewer.UI.media != null)
                {
                    if (isSliderBeingDragged&&_notResponsing)
                    {
                        new Thread(() =>
                        {
                            _notResponsing = false;
                            Thread.Sleep(500);
                            if (Mouse.LeftButton == MouseButtonState.Released)
                            {
                                Dispatcher.Invoke(new MyAction(() =>
                                {
                                    VideoViewer.UI.media.Position = TimeSpan.FromSeconds((sender as Slider).Value);
                                }));
                            }
                            _notResponsing = true;
                        }).Start();
                    }
                }
            }
            else
            {

                if (_media1 != null)
                {
                    if (isSliderBeingDragged && _notResponsing)
                    {
                        new Thread(() =>
                        {
                            _notResponsing = false;
                            Thread.Sleep(500);
                                Dispatcher.Invoke(new MyAction(() =>
                                {
                                if (Mouse.LeftButton == MouseButtonState.Released)
                                {
                                    _media1.Position = TimeSpan.FromSeconds((sender as Slider).Value);
                                    }
                                }));
                            
                            _notResponsing = true;
                        }).Start();
                    }
                }
            }
            // 获取当前 Slider 的 Template
            var slider = sender as Slider;
                // 使用 Template.FindName 获取 PART_FILLED_RECTANGE
                Rectangle filledPart = slider.Template.FindName("PART_FILLED_RECTANGE", slider) as Rectangle;
                if (filledPart != null)
                {
                    // 计算已走过区域的宽度
                    double value = slider.Value;
                    double maximum = slider.Maximum;
                    double width = slider.ActualWidth;

                    // 计算并设置 Rectangle 的宽度
                    filledPart.Width = (value / maximum) * width;
                
            }
        }

        private void priviousBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {

            var i = Models.Lists.Playing.IndexOf(_currentFile);
            if(i - 1 < 0)
            {
                i = Models.Lists.Playing.Count - 1;
            }
            else
            {
                i--;
            }
            var path = Models.Lists.Playing[ i% Models.Lists.Playing.Count];

            if (_currentFile.EndsWith(".mp4"))
            {
                PlayVedio(path);
            }
            else
            {
                playMusic(path);
            }
        }

        internal void PlayBtnGroup_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UIPlay(_playingVedio);
        }

        internal void stopBtnGroup_MouseDown(object sender, MouseButtonEventArgs e)
        {
            pause();
            stopBtnGroup.Visibility = Visibility.Hidden;
            PlayBtnGroup.Visibility = Visibility.Visible;
        }

        private void NextBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var i = Models.Lists.Playing.IndexOf(_currentFile);
            var path = Models.Lists.Playing[(i + 1) % Models.Lists.Playing.Count];
            if (ca == CompetedAction.Randomly)
            {
                var rand = new Random().Next(0, Models.Lists.Playing.Count-1);
                while ( rand == i&& Models.Lists.Playing.Count>1)
                {
                    rand = new Random().Next(0, Models.Lists.Playing.Count - 1);
                }
                path = Models.Lists.Playing[rand];
            }
            if (_currentFile.EndsWith(".mp4"))
            {
                playVedio(path);
            }
            else
            {
                playMusic(path);
            }
        }

        private void Vol_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (
            VolCanvas.Visibility == Visibility.Visible)
            {
                VolCanvas.Visibility = Visibility.Hidden;
            }
            else
            {
                VolCanvas.Visibility = Visibility.Visible;
            }
            VolGrid.SetValue(Canvas.TopProperty, -VolGrid.Height+20);
            VolGrid.SetValue(Canvas.LeftProperty, this.ActualWidth - 80);
        }

        private void List_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PlayList.Current.Show();
        }

        private void SeekToBar_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // 获取 Slider
            Slider slider = sender as Slider;

            // 获取鼠标点击位置
            Point mousePosition = e.GetPosition(slider);

            // 计算 Slider 的宽度
            double sliderWidth = slider.ActualWidth;

            // 计算 Slider 的值
            double value = (mousePosition.X / sliderWidth) * (slider.Maximum - slider.Minimum) + slider.Minimum;

            isSliderBeingDragged = true;
            // 设置 Slider 的值
            slider.Value = Math.Max(slider.Minimum, Math.Min(slider.Maximum, value));
            isSliderBeingDragged=false;

            SeekToBar_MouseDown(sender,e);
        }

        private void VolS_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_playingVedio)
            {

                if (VideoViewer.UI.media != null)
                {
                    VideoViewer.UI.media.Volume = (float)(VolS.Value / 100d);
                }
            }
            else
            {
                if (this._media1 != null)
                {
                    _media1.Volume = (float)(VolS.Value / 100d);
                }
            }
            if (VolT == null) return;
            if(VolS.Value == 100)
            {
                VolT.Text = "100";
            }
            else if (VolS.Value.ToString().Length>1)
            {
                VolT.Text = (VolS.Value).ToString().Substring(0, 2);
            }
            else
            {
                VolT.Text = (VolS.Value).ToString().Substring(0, 1);
            }
        }

        internal static void PlayVedio(string mp4)
        {
            Current.playVedio(mp4);
        }

        private void playVedio(string mp4)
        {
            _playingVedio = true;
            _currentFile = mp4;
            using (var file = TagLib.File.Create(_currentFile))
            {
                _tag = file.Tag;
            }
            TitleBox.Text = _tag.Title;
            AlbumNameBox.Visibility = Visibility.Hidden;
            ASeprector.Visibility = Visibility.Hidden;
            foreach (var item in _tag.Performers)
            {
                PerformersBox.Text += item;
            }
            if (VideoViewer.UI.media == null)
            {
                VideoViewer.UI.media = new MediaElement();
                VideoViewer.UI.media.LoadedBehavior = MediaState.Manual;
                VideoViewer.UI.media.UnloadedBehavior = MediaState.Manual;
            }
            stop();
            VideoViewer.UI.media.Source = new Uri(mp4);
            VideoViewer.UI.media.Volume = (float)(VolS.Value / 1d);
            _currentFile = mp4;

            UIPlay(true);
        }

        private void SeekToBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            new Thread(() =>
            {
                Thread.Sleep(200);
                bool boed = false;
                Dispatcher.Invoke(new MyAction(() =>
                {
                    if (Mouse.LeftButton == MouseButtonState.Released)
                    {
                        boed = true;
                    }
                }));
                if (boed)
                {

                }
                else if (isSliderBeingDragged)
                {
                    return;
                }
                if (_playingVedio)
                {
                    if (VideoViewer.UI.media != null)
                    {
                        Dispatcher.Invoke(new MyAction(() =>
                        {
                            VideoViewer.UI.media.Position = TimeSpan.FromSeconds((sender as Slider).Value);
                        }));
                    }
                }
                else
                {

                    if (_media1 != null)
                    {

                        Dispatcher.Invoke(new MyAction(() =>
                        {
                            _media1.Position = TimeSpan.FromSeconds((sender as Slider).Value);
                        }));
                    }
                                
                }
            }).Start();
        }

        internal static void Hide()
        {
            if (Current.Margin.Bottom == 0)
            {
                if (Current.EnableAnimAndEvent)
                {
                    Current.Hidden = true;
                    Current.BeginAnimation(MarginProperty, MarginAnim2);
                    Current.BeginAnimation(VisibilityProperty, BooledAnim());
                }
            }
        }

        private void RepeatBe_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ca == CompetedAction.RepeatList)
            {
                RepeatPNG.Visibility = Visibility.Hidden;
                RepeatOnePNG.Visibility = Visibility.Visible;
                ca = CompetedAction.RepeatOne;
            }
            else if (ca == CompetedAction.RepeatOne)
            {

                RepeatOnePNG.Visibility = Visibility.Hidden;
                RandomlyPNG.Visibility = Visibility.Visible;
                ca = CompetedAction.Randomly;
            }
            else if (ca == CompetedAction.Randomly)
            {

                RandomlyPNG.Visibility = Visibility.Hidden ;
                RepeatPNG.Visibility = Visibility.Visible;
                ca = CompetedAction.RepeatList;
            }
        }
    }
    public enum CompetedAction
    {
        RepeatList,RepeatOne,Randomly
    }
}
