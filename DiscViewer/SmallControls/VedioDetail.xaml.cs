using DiscViewer.Views;
using Dtimu.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

namespace DiscViewer.SmallControls
{
    /// <summary>
    /// VedioDetail.xaml 的交互逻辑
    /// </summary>
    public partial class VedioDetail : UserControl
    {
        public VedioDetail()
        {
            InitializeComponent();

            this.DataContextChanged += VedioDetail_DataContextChanged;
        }
        List<System.Windows.Controls.Image> imas = new List<System.Windows.Controls.Image>();
        private void VedioDetail_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is Video v)
            {
                var tot = v.Gallery.Count;
                foreach (var item in v.Gallery)
                {
                    imas.Add(new System.Windows.Controls.Image() { Source = ConvertPictrueDataToBitmapImage(System.Convert.FromBase64String(item)) });
                }
                var inde = (int)Math.Ceiling(tot / 2d);
                GalleryBorder.Child = null;
                GalleryBorder.Child = imas[inde];
            }
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var dc = (Dtimu.Core.Video)DataContext;
            VideoViewer.Show(dc);
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            var easing = new PowerEase()
            {
                EasingMode = EasingMode.EaseInOut
            };
            DoubleAnimation zoomInX = new DoubleAnimation(BackGroundRect.Opacity, 0.2, TimeSpan.FromSeconds(0.2))
            {
                EasingFunction = easing
            };
            BackGroundRect.BeginAnimation(OpacityProperty, zoomInX);
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {

            var easing = new PowerEase()
            {
                EasingMode = EasingMode.EaseInOut
            };
            DoubleAnimation zoomInX = new DoubleAnimation(BackGroundRect.Opacity, 0d, TimeSpan.FromSeconds(0.2))
            {
                EasingFunction = easing
            };
            BackGroundRect.BeginAnimation(OpacityProperty, zoomInX);
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

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            var position = Mouse.GetPosition(sender as IInputElement);

            var video = DataContext as Video;
            if (video == null || video.Gallery == null || video.Gallery.Count == 0) return;

            // 计算当前鼠标位置对应的索引
            double itemWidth = 160d;
            int index = (int)Math.Ceiling((position.X / itemWidth) * video.Gallery.Count) - 1;

            // 保证 index 在有效范围内
            index = Math.Max(0, Math.Min(index, imas.Count - 1));

            // 更新 UI
            GalleryBorder.Child = null;
            GalleryBorder.Child = imas[index];
        }
    }
}
