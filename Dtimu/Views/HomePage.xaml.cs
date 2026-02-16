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
using Dtimu.Controls;
using System.Collections.ObjectModel;
using Dtimu.Models;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Dtimu.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class HomePage : Page
    {

        public HomePage()
        {
            this.InitializeComponent();

            if (GlobalConfigs.RecentItems.Count == 0)
            {
                BigSP.Children.Remove(RecentItemsSP);
            }
            RIP.RecentItems = GlobalConfigs.RecentItems;
            GlobalConfigs.ServerInstances = new ObservableCollection<ServerInstance>();
            Utils.RemoteUtil.StartFindServers();
            SEP.ServerInstances = GlobalConfigs.ServerInstances;

            
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            
                MainPage.CFM.Navigate(typeof(SettingsPage),
                    null, new
                    DrillInNavigationTransitionInfo());
            
        }
    }
}
