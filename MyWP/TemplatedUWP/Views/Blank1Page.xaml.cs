using System;

using TemplatedUWP.ViewModels;

using Windows.UI.Xaml.Controls;

namespace TemplatedUWP.Views
{
    public sealed partial class Blank1Page : Page
    {
        private Blank1ViewModel ViewModel
        {
            get { return ViewModelLocator.Current.Blank1ViewModel; }
        }

        public Blank1Page()
        {
            InitializeComponent();
        }
    }
}
