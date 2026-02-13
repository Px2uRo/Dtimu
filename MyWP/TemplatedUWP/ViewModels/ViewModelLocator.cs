using System;

using GalaSoft.MvvmLight.Ioc;

using TemplatedUWP.Services;
using TemplatedUWP.Views;

namespace TemplatedUWP.ViewModels
{
    [Windows.UI.Xaml.Data.Bindable]
    public class ViewModelLocator
    {
        private static ViewModelLocator _current;

        public static ViewModelLocator Current => _current ?? (_current = new ViewModelLocator());

        private ViewModelLocator()
        {
            SimpleIoc.Default.Register(() => new NavigationServiceEx());
            SimpleIoc.Default.Register<ShellViewModel>();
            Register<MainViewModel, MainPage>();
            Register<BlankViewModel, BlankPage>();
            Register<Blank1ViewModel, Blank1Page>();
            Register<MediaPlayerViewModel, MediaPlayerPage>();
            Register<SchemeActivationSampleViewModel, SchemeActivationSamplePage>();
        }

        public SchemeActivationSampleViewModel SchemeActivationSampleViewModel => SimpleIoc.Default.GetInstance<SchemeActivationSampleViewModel>();

        // A Guid is generated as a unique key for each instance as reusing the same VM instance in multiple MediaPlayerElement instances can cause playback errors
        public MediaPlayerViewModel MediaPlayerViewModel => SimpleIoc.Default.GetInstance<MediaPlayerViewModel>(Guid.NewGuid().ToString());

        public Blank1ViewModel Blank1ViewModel => SimpleIoc.Default.GetInstance<Blank1ViewModel>();

        public BlankViewModel BlankViewModel => SimpleIoc.Default.GetInstance<BlankViewModel>();

        public MainViewModel MainViewModel => SimpleIoc.Default.GetInstance<MainViewModel>();

        public ShellViewModel ShellViewModel => SimpleIoc.Default.GetInstance<ShellViewModel>();

        public NavigationServiceEx NavigationService => SimpleIoc.Default.GetInstance<NavigationServiceEx>();

        public void Register<VM, V>()
            where VM : class
        {
            SimpleIoc.Default.Register<VM>();

            NavigationService.Configure(typeof(VM).FullName, typeof(V));
        }
    }
}
