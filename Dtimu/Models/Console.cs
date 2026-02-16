using System;
using System.Diagnostics;
using Windows.UI.Popups;

namespace Dtimu.Models
{
    internal class Console
    {
        internal static void WriteLine(string v)
        {
            MessageDialog dialog = new MessageDialog(v);
            dialog.ShowAsync().GetResults();
        }
    }
}