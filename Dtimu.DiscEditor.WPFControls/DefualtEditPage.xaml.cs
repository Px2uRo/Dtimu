using Dtimu.DiscEditor.Core;
using System;
using System.Collections.Generic;
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

namespace Dtimu.DiscEditor.WPFControls
{
    /// <summary>
    /// DefualtEditPage.xaml 的交互逻辑
    /// </summary>
    public partial class DefualtEditPage : UserControl
    {
        public Project WorkingProj { get; set; }
        public string WorkingPath { get; set; }
        public DefualtEditPage()
        {
            InitializeComponent();
        }

        public DefualtEditPage(Project project)
        {
            this.Resources["CurrentProj"] = project;
            InitializeComponent();
            WorkingProj = project;
            MEDIA.ItemsSource = WorkingProj.MusicProjects;
            MEDIA_VIDEO.ItemsSource = WorkingProj.VideoProjects;
        }

        private void MUSIC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var s1 = (sender as ListBox).SelectedItem;
            DetailedINFO.DataContext = s1; 
            ValidateAllTextBoxesWithBinding(this);
            e.Handled = true;
        }

        public bool HasErrors { get; private set; } = false;
        private void ValidateAllTextBoxesWithBinding(DependencyObject parent)
        {
            // 重置 HasErrors 为 false
            HasErrors = false;

            // 获取子控件的数量
            int childCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childCount; i++)
            {
                // 获取子控件
                var child = VisualTreeHelper.GetChild(parent, i);

                // 检查子控件是否为 TextBox 并且具有绑定
                if (child is TextBox textBox)
                {
                    // 获取 TextBox 的 BindingExpression
                    var binding = textBox.GetBindingExpression(TextBox.TextProperty);

                    // 如果有绑定，触发验证
                    binding?.UpdateSource();

                    // 检查是否有验证错误
                    if (Validation.GetHasError(textBox))
                    {
                        HasErrors = true;
                    }
                }

                // 递归检查子控件
                ValidateAllTextBoxesWithBinding(child);
            }
        }
        private void Run_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void LeftMenus_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (filePaths.Length > 0)
                {
                    foreach (var file in filePaths)
                    {
                        if (Dtimu.DiscEditor.Core.CodeSetting.MusicExtensions.Any(ext => file.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                        {
                            WorkingProj.MusicProjects.Add(MusicFileProjInfo.OpenFile(file));
                        }
                        else if (Dtimu.DiscEditor.Core.CodeSetting.VideoExtensions.Any(ext => file.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                        {
                            WorkingProj.VideoProjects.Add(VideoFileProjInfo.OpenFile(file));
                        }
                    }
                }
            }
        }

        private void Export_TO_ISO_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Export_TO_Folder_Click(object sender, RoutedEventArgs e)
        {
            WPFControls.Utils.FolderDia.Reset();
            WPFControls.Utils.FolderDia.ShowNewFolderButton = true;
            WPFControls.Utils.FolderDia.Description = "选择目标文件夹";
            WPFControls.Utils.FolderDia.ShowDialog();
            if (string.IsNullOrEmpty(WPFControls.Utils.FolderDia.SelectedPath))
            {
                return;
            }
            Dtimu.DiscEditor.Core.Utils.ExportToFolder(this.WorkingProj, WPFControls.Utils.FolderDia.SelectedPath);
        }

        private void SAVE_PROJ_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(WorkingPath))
            {

                WPFControls.Utils.SavFileDia.Reset();
                WPFControls.Utils.SavFileDia.FileName = CodeSetting.ProjExName;
                WPFControls.Utils.SavFileDia.Title = "选择文件路径";
                WPFControls.Utils.SavFileDia.ShowDialog();
                if (string.IsNullOrEmpty(WPFControls.Utils.SavFileDia.FileName))
                {
                    return;
                }
                WorkingPath = WPFControls.Utils.SavFileDia.FileName;
            }
            WorkingProj.Save(WorkingPath);
        }

        private void TextBox_Error(object sender, ValidationErrorEventArgs e)
        {
            var d = DetailedINFO.DataContext as MusicFileProjInfo;
            if (e.Action==ValidationErrorEventAction.Added)
            {
                d.NoError = false;
                return;
            }
            else
            {
                ValidateAllTextBoxesWithBinding(this);
                d.NoError = !HasErrors;

            }
            // 强制刷新 ListBox 的 ItemsSource
            var view = CollectionViewSource.GetDefaultView(MEDIA.ItemsSource);
            view.Refresh();
        }

        private void AddSubtiles_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MEDIA_VIDEO_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
