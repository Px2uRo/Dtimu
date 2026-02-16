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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Dtimu.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MusicListView : Page
    {
        public ServerInstance ServerInstance { get; set; }
        public MusicListView()
        {
            this.InitializeComponent();
            DataContext = GlobalConfigs.CurrentServerInstance.DireInfo;
            RefreshList();
        }

        private void RefreshList()
        {
            var di = DataContext as DireInfo;
            var ml = new List<Music>();
            foreach (var al in di.Albums)
            {
                foreach (var item in al.Value.Musics)
                {
                    ml.Add(di.Musics[item]);
                }
            }
            ml = ml.OrderBy(x=>x.Title).ToList();
            MusicsLB.ItemsSource = ml;
        }

        private void MusicsLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selection = MusicsLB.SelectedItem;

            GlobalConfigs.AddToPlayingList(selection as Music);
            MainPage.CFM.Navigate(typeof(MusicPage),
                        null, new
                        DrillInNavigationTransitionInfo());
        }
    }
}
