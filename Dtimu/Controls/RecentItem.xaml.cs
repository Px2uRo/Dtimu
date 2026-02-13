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

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace Dtimu.Controls
{
    public sealed partial class RecentItem : UserControl
    { 
        public RecentItem()
        {
            this.InitializeComponent();
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value);
            }
        }
        

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(

                "Title", 
                typeof(string), 
                typeof(RecentItem), 
                new PropertyMetadata("Defualt_Title", (d,e)=> {
                    var r = (d as RecentItem);
                    r.PART_Title.Content = e.NewValue;
                })
        );
    }
}
