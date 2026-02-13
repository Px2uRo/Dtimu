using System;

using TemplatedUWP.ViewModels;

using Windows.UI.Xaml.Controls;

namespace TemplatedUWP.Views
{
    public sealed partial class MainPage : Page
    {
        private MainViewModel ViewModel
        {
            get { return ViewModelLocator.Current.MainViewModel; }
        }

        public MainPage()
        {
            InitializeComponent();
        }
    }
}
