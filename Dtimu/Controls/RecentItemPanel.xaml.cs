using Dtimu.Models;
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
using Windows.UI.Xaml.Navigation;
using Windows.System.Profile;
using Windows.UI.ViewManagement;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace Dtimu.Controls
{
    public sealed partial class RecentItemPanel : UserControl
    {
        public RecentItemPanel()
        {
            this.Loaded += RecentItemPanel_Loaded;
            this.InitializeComponent();
        }

        private void RecentItemPanel_Loaded(object sender, RoutedEventArgs e)
        {
            if (AnalyticsInfo.VersionInfo.DeviceFamily.Contains("Mobile"))
            {
                SCRLV.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            }
        }

        public IEnumerable<RecentItemPair> RecentItems
        {
            get { return (IEnumerable<RecentItemPair>)GetValue(RecentItemsProperty); }
            set { SetValue(RecentItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RecentItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RecentItemsProperty =
            DependencyProperty.Register("RecentItems", typeof(IEnumerable<RecentItemPair>), typeof(RecentItemPanel),
                new PropertyMetadata(null, (d, e) =>
                {
                    var r = (d as RecentItemPanel);
                    r.LV.ItemsSource = e.NewValue;
                }));


    }

}
