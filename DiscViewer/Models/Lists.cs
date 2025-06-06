using System.Collections.ObjectModel;

namespace DiscViewer.Models
{
    internal static class Lists
    {
        public static ObservableCollection<string> Playing { get; set; } = new ObservableCollection<string>();
    }
}