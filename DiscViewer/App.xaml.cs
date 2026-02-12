using DiscViewer.Views;
using Dtimu.IndexSchemas;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DiscViewer
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
#if DEBUG
            TargetDire = "I:\\yqq";
            //TargetDire = "J:\\Hanime";
            //TargetDire = "I:\\SHAGGY_SU";
            //TargetDire = "J:\\Box\\迷宫河";
            //TargetDire = "I:\\前年照片 - 副本";

#elif !DEBUG
            var ar1 = e.Args[0]; TargetDire = ar1;
#endif
            var ip = System.IO.Path.Combine(App.TargetDire, "index.json");
            if (System.IO.File.Exists(ip))
            {
                var json = System.IO.File.ReadAllText(ip);
                DireInfo = JsonConvert.DeserializeObject<DireInfo>(json);
            }
            EventManager.RegisterClassHandler(typeof(Window), UIElement.KeyDownEvent, new KeyEventHandler(KeyDown));
            base.OnStartup(e);
        }

        private void KeyDown(object sender, KeyEventArgs e)
        {
            if (PlayingBar.Current.Visibility == Visibility.Hidden)
            {
                return;
            }
            if(e.Key==Key.Enter)
            {
                var mv = MainWindow as DiscViewer.MainWindow;
                if(mv.WindowState == WindowState.Maximized)
                {
                    mv.ExitFullScreen();
                }
                else
                {
                    mv.GoFullScreen();
                }
            }
            else if (e.Key == Key.Space)
            {
                if (PlayingBar.Current.Stopped)
                {
                    return;
                }
                if (PlayingBar.Current._isPaused)
                {
                    PlayingBar.Current.PlayBtnGroup_MouseDown(this,null);
                }
                else
                {
                    PlayingBar.Current.stopBtnGroup_MouseDown(this, null);
                }
            }
            else if (e.Key == Key.Left)
            {
                if (PlayingBar._playingVedio)
                {
                    if (VideoViewer.UI.media != null)
                    {
                        double newPosition = PlayingBar.Current.SeekToBar.Value - 10;
                        newPosition = Math.Max(0, Math.Min(newPosition, PlayingBar.Current.SeekToBar.Maximum));
                        VideoViewer.UI.media.Position = TimeSpan.FromSeconds(newPosition);
                    }
                }
                else
                {
                    if (PlayingBar.Current._media1 != null)
                    {
                        double newPosition = PlayingBar.Current.SeekToBar.Value - 10;
                        newPosition = Math.Max(0, Math.Min(newPosition, PlayingBar.Current.SeekToBar.Maximum));
                        PlayingBar.Current._media1.Position = TimeSpan.FromSeconds(newPosition);
                    }
                }
            }
            else if (e.Key == Key.Right)
            {

                if (PlayingBar._playingVedio)
                {
                    if (VideoViewer.UI.media != null)
                    {
                        double newPosition = PlayingBar.Current.SeekToBar.Value + 10;
                        newPosition = Math.Max(0, Math.Min(newPosition, PlayingBar.Current.SeekToBar.Maximum));
                        VideoViewer.UI.media.Position = TimeSpan.FromSeconds(newPosition);
                    }
                }
                else
                {
                    if (PlayingBar.Current._media1 != null)
                    {
                        double newPosition = PlayingBar.Current.SeekToBar.Value + 10;
                        newPosition = Math.Max(0, Math.Min(newPosition, PlayingBar.Current.SeekToBar.Maximum));
                        PlayingBar.Current._media1.Position = TimeSpan.FromSeconds(newPosition);
                    }
                }
            }
            else if (e.Key == Key.Escape)
            {
                EscapeBahavior.Invoke(EscapeSender, e);
            }
        }

        public static ObservableCollection<string> PlayingList { get; set; } = new ObservableCollection<string>();
        public static DireInfo DireInfo { get; internal set; }
        public static string TargetDire { get; internal set; }
        public object EscapeSender { get; set; }
        public event EventHandler<EventArgs> EscapeBahavior;
    }
}
