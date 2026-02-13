using Dtimu.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace Dtimu.Controls
{
    public sealed partial class ServerEntrancePanel : UserControl
    {

        public IEnumerable<ServerInstance> ServerInstances
        {
            get { return (IEnumerable<ServerInstance>)GetValue(ServerInstancesProperty); }
            set { SetValue(ServerInstancesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RecentItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ServerInstancesProperty =
            DependencyProperty.Register("ServerInstances", typeof(IEnumerable<ServerInstance>), typeof(ServerEntrancePanel),
                new PropertyMetadata(null
                    , (d, e) =>
                {
                    var r = (d as ServerEntrancePanel);
                    r.LV.ItemsSource = e.NewValue;
                }
                ));
        

        public ServerEntrancePanel()
        {
            this.InitializeComponent();
        }
    }
}
