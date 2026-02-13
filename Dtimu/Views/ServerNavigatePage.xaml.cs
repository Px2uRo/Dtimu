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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Dtimu.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ServerNavigatePage : Page
    {
        public ServerNavigatePage()
        {
            this.InitializeComponent();
        }

        private void MenuList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MenuList.SelectedItem is ListBoxItem item)
            {
                Type targetPage = null;

                switch (item.Tag)
                {
                    case "Video":
                        targetPage = typeof(VideoPage);
                        break;
                    case "Music":
                        targetPage = typeof(MusicListView);
                        break;
                    case "Pictures":
                        targetPage = typeof(PicturePage);
                        break;
                }

                if (targetPage != null)
                {
                    MainPage.CFM.Navigate(targetPage,
                        null, new
                        DrillInNavigationTransitionInfo());
                }
            }
        }
    }
}
