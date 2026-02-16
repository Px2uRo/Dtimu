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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Dtimu.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        TextBox AddTB = new TextBox() { Margin=new Thickness(10,0,0,0)};
        public SettingsPage()
        {
            this.InitializeComponent();

            RecordedDevices.ItemsSource = GlobalConfigs.RecordedDevices;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Grd.Children.Add(AddTB);
            AddBtn.SetValue(Grid.ColumnProperty, 2);
            (sender as Button).Click -= Button_Click;
            (sender as Button).Click += SettingsPage_Click; ;

        }

        private void SettingsPage_Click(object sender, RoutedEventArgs e)
        {
            var text = AddTB.Text?.Trim();

            if (string.IsNullOrWhiteSpace(text))
                return;

            var normalized = NormalizeEndpoint(text);

            // 防止重复添加
            if (!GlobalConfigs.RecordedDevices.Contains(normalized))
            {
                GlobalConfigs.RecordedDevices.Add(normalized);
            }
            AddTB.Text = string.Empty;
        }
        private void RecordedDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selection = RecordedDevices.SelectedItem as string;

            GlobalConfigs.RecordedDevices.Remove(selection);
        }

        private string NormalizeEndpoint(string input)
        {
            input = input.Trim();
            
            if (input.StartsWith("["))
            {
                if (input.Contains("]:"))
                    return input; 

                return input + ":26131";
            }

            // 统计冒号数量
            int colonCount = input.Count(c => c == ':');

            // 没有冒号 → IPv4 或域名
            if (colonCount == 0)
                return input + ":26131";
            
            if (colonCount == 1)
                return input;
            
            return "[" + input + "]:26131";
        }
    }
}
