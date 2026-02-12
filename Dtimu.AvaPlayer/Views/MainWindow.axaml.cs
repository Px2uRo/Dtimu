using Avalonia.Controls;
using Dtimu.AvaPlayer.Controls;

namespace Dtimu.AvaPlayer.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var cont = new VideoControl(); this.Content = cont;

        cont.Play("https://storage.home/files/Vedios/Passport.mp4");
    }


}
