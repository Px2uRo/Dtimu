using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace Dtimu.Controls
{
    public sealed partial class Marquee : UserControl
    {
        public Marquee()
        {
            this.InitializeComponent();
            //this.Loaded += Page_Loaded;
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(Marquee), new PropertyMetadata(string.Empty,
                (d,e)=> {
                    var r = (d as Marquee);
                    r.MarqueeText.Text = (string)e.NewValue;
                }));

        Storyboard _marqueeStoryboard;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MarqueeHost.SizeChanged += (_, __) => StartMarquee();
            StartMarquee();
        }

        private void StartMarquee()
        {
            if (_marqueeStoryboard != null)
            {
                _marqueeStoryboard.Stop();
            }

            MarqueeText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            double textWidth = MarqueeText.DesiredSize.Width;
            double hostWidth = MarqueeHost.ActualWidth;

            if (textWidth <= hostWidth)
                return; // 不需要跑

            var transform = new TranslateTransform();
            MarqueeText.RenderTransform = transform;

            double from = hostWidth;
            double to = -textWidth;

            var animation = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = TimeSpan.FromSeconds(10),
                RepeatBehavior = RepeatBehavior.Forever
            };

            Storyboard.SetTarget(animation, transform);
            Storyboard.SetTargetProperty(animation, "X");

            _marqueeStoryboard = new Storyboard();
            _marqueeStoryboard.Children.Add(animation);

            _marqueeStoryboard.Begin();
        }
    }
}
