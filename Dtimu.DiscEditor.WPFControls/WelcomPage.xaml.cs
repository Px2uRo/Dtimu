using Dtimu.DiscEditor.Core;
using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TagLib.Ape;

namespace Dtimu.DiscEditor.WPFControls
{
    /// <summary>
    /// WelcomePage.xaml 的交互逻辑
    /// </summary>
    public partial class WelcomePage : UserControl
    {
        public WelcomePage()
        {
            InitializeComponent();
        }

        private void Templates_Loaded(object sender, RoutedEventArgs e)
        {
            var st = sender as StackPanel;
            foreach (var item in CodeSetting.GetDefalutTemplates())
            {
                if (item.Page == null)
                {
                    item.Page = new Dtimu.DiscEditor.WPFControls.DefualtEditPage(new Project());
                }
                var img = new Image() { Height = 90, Width = 90, Source = Utils.ConvertBase64ToBitmapImage(item.Icon) };
                var con = new StackPanel();
                con.Children.Add(img);
                con.Children.Add(new TextBlock() { Text = item.Name });
                con.MouseDown += (s, e2) =>
                {
                    var mw = Application.Current.MainWindow;
                    mw.Content = item.Page;
                };
                st.Children.Add(con);
            }
        }

        private void UserControl_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (filePaths.Length > 0)
                {
                    string filePath = filePaths[0];
                    var mw = Application.Current.MainWindow;
                    mw.Content = Utils.GetProjPage(filePath);
                }
            }
        }

        private void RecentItemsStack_Loaded(object sender, RoutedEventArgs e)
        {
            string recentFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Recent));

            foreach (var filePath in Directory.GetFiles(recentFolder))
            {
                if (filePath.EndsWith(CodeSetting.ProjExName+".lnk"))
                {
                    var s = sender as StackPanel;
                    var ele = new TextBlock { Background = new SolidColorBrush(Colors.Transparent),
                        Text = filePath };
                    ele.MouseDown += Ele_MouseDown;
                    s.Children.Add(ele);
                }
            }
        }

        private void Ele_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var t = sender as TextBlock;
            var shell = new WshShell();
            var shortcut = (IWshShortcut)shell.CreateShortcut(t.Text);
            var mw = Application.Current.MainWindow;
            mw.Content = Utils.GetProjPage(shortcut.TargetPath);


        }
    }
}
