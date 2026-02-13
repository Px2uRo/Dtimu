using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Dtimu.Views
{
    public sealed partial class MainPage : Page
    {
        public static Frame CFM;
        public MainPage()
        {
            this.InitializeComponent();

            // 注册系统返回请求
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

            // 初始显示主页面
            ContentFrame.Navigate(typeof(HomePage));

            CFM = ContentFrame;
        }


        // 系统返回键逻辑
        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (ContentFrame.CanGoBack)
            {
                e.Handled = true;
                ContentFrame.GoBack();
            }
        }

        // 每次导航完成时，更新菜单显示
        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            // 只有 HomePage 显示菜单，其他页面隐藏
            if (e.Content is HomePage)
            {
                SystemNavigationManager.GetForCurrentView()
                    .AppViewBackButtonVisibility =
 AppViewBackButtonVisibility.Collapsed;

            }
            else
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                AppViewBackButtonVisibility.Visible;
            }
            
        }
    }
}
