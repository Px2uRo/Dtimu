using System;

using Avalonia;
using Microsoft.Extensions.DependencyInjection;

namespace Dtimu.AvaPlayer.Desktop;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {

        var serviceCollection = new ServiceCollection();
        InitCollection(serviceCollection);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        CommunityToolkit.Mvvm.DependencyInjection.Ioc.Default.ConfigureServices(serviceProvider);

        return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
    }

    private static void InitCollection(ServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<Dtimu.AvaPlayer.Singletons.IFFmpegInfo, FFmpegInfo>();
    }
}
