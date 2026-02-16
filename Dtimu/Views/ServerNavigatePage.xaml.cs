using Dtimu.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Dtimu.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ServerNavigatePage : Page
    {
        public List<BitmapImage> images = new List<BitmapImage>();
        public ServerNavigatePage()
        {
            this.InitializeComponent();
            DataContext = GlobalConfigs.CurrentServerInstance;
#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
            MusicsBitmapAnim();
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
                              //LoadPicturesAsyncs();
        }

        private async Task MusicsBitmapAnim()
        {
            var di = (DataContext as ServerInstance).DireInfo;
            bool ensure = false;
            if (!(di.Pictures.Count < 9))
            {
                ensure = true;
            }
            if (!ensure) return;
            var al = di.Albums.Values.ToList();
            var imaC = new List<Windows.UI.Xaml.Controls.Image>() { Img1, Img2, Img3, Img4, Img5, Img6, Img7, Img8, Img9 };
            for (int i = 0; i < 9; i++)
            {
                var hash = al[i].Pictures.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(hash))
                {
                    var b64 = di.Pictures[hash];
                    if (!string.IsNullOrWhiteSpace(b64))
                    {
                        var sour = await GetBitmapAsyncs(b64);
                        await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            imaC[i].Source = sour;
                            imaC[i].Projection = new PlaneProjection();
                        });
                    }
                }
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    imaC[i].Projection = new PlaneProjection();
                });

            }
            while (ensure)
            {
                for (int i = 9; i < al.Count; i++)
                {
                    var hash = al[i].Pictures.FirstOrDefault();
                    if (string.IsNullOrWhiteSpace(hash))
                    {
                        continue;
                    }
                    var b64 = di.Pictures[hash];
                    if (!string.IsNullOrWhiteSpace(b64))
                    {
                        await Task.Delay(2000);
                        var sour = await GetBitmapAsyncs(b64);
                        var inde = i  % 9;
                        await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                        {
                            var proj = imaC[inde].Projection as PlaneProjection;

                            var sb = new Storyboard();
                            var anim = new DoubleAnimationUsingKeyFrames();
                            Storyboard.SetTarget(anim, proj);
                            Storyboard.SetTargetProperty(anim, "RotationX");
                            
                            anim.KeyFrames.Add(new SplineDoubleKeyFrame
                            {
                                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(500)),
                                Value = 90,
                                KeySpline = 
                                new KeySpline
                                { ControlPoint1 = new Windows.Foundation.Point(0.33, 0.0),
                                    ControlPoint2 = new Windows.Foundation.Point(0.68, 1.0) }
                            });
                            
                            anim.KeyFrames.Add(new DiscreteDoubleKeyFrame
                            {
                                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(501)),
                                Value = -90
                            });
                            anim.KeyFrames.Add(new EasingDoubleKeyFrame
                            {
                                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(850)),
                                Value = 20,
                                EasingFunction = new BackEase
                                {
                                    Amplitude = 0.35,
                                    EasingMode = EasingMode.EaseOut
                                }
                            });
                            
                            anim.KeyFrames.Add(new EasingDoubleKeyFrame
                            {
                                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(1000)),
                                Value = 0,
                                EasingFunction = new CubicEase
                                {
                                    EasingMode = EasingMode.EaseOut
                                }
                            });

                            sb.Children.Add(anim);
                            sb.Begin();
                            
                            await Task.Delay(500);
                            imaC[inde].Source = sour;
                        });
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }

        public async Task<BitmapImage> GetBitmapAsyncs(string base64)
        {
            var bytes = Convert.FromBase64String(base64);

            using (var stream = new InMemoryRandomAccessStream())
            {
                await stream.WriteAsync(bytes.AsBuffer());
                stream.Seek(0);

                var bmp = new BitmapImage();
                await bmp.SetSourceAsync(stream);

                return bmp;
            }
        }

        //private async Task LoadPicturesAsyncs()
        //{

        //}

        private void MenuList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (MenuList.SelectedItem is ListBoxItem item)
            //{
            //    Type targetPage = null;

            //    switch (item.Tag)
            //    {
            //        case "Video":
            //            targetPage = typeof(VideoPage);
            //            break;
            //        case "Music":
            //            targetPage = typeof(MusicListView);
            //            break;
            //        case "Pictures":
            //            targetPage = typeof(PicturePage);
            //            break;
            //    }

            //    if (targetPage != null)
            //    {
            //        MainPage.CFM.Navigate(targetPage,
            //            null, new
            //            DrillInNavigationTransitionInfo());
            //    }
            //}
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainPage.CFM.Navigate(typeof(MusicListView),
                        null, new
                        DrillInNavigationTransitionInfo());
        }
    }
}
