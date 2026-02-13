using System;

using TemplatedUWP.ViewModels;

using Windows.UI.Xaml.Controls;

namespace TemplatedUWP.Views
{
    public sealed partial class BlankPage : Page
    {
        private BlankViewModel ViewModel
        {
            get { return ViewModelLocator.Current.BlankViewModel; }
        }

        public BlankPage()
        {
            InitializeComponent();
        }
    }
}
