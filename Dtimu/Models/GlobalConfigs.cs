using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using System.Collections.ObjectModel;
using Dtimu.Utils;
using System.Net;
using Dtimu.Controls;
using Windows.UI.Xaml.Controls;
using Dtimu.Views;

namespace Dtimu.Models
{
    public static class GlobalConfigs
    {
        public static ObservableCollection<ServerInstance> ServerInstances { get; set; }
        public static ObservableCollection<RecentItemPair> RecentItems { get; set; }
        public static ServerInstance CurrentServerInstance { get; internal set; }
        public static string CurrentHash { get; set; }

        public static void Initialize()
        {
            RecentItems = new ObservableCollection<RecentItemPair>();

            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            String recentItemsJson = localSettings.Values["RecentItems"] as String;
            if (!string.IsNullOrWhiteSpace(recentItemsJson))
            {
                if (JsonArray.TryParse(recentItemsJson, out var riIns))
                {
                    foreach (var jO in riIns)
                    {
                        var pair = new RecentItemPair();
                        pair.Title = JsonObject.Parse(jO.Stringify())["title"].ToString();
                        
                        RecentItems.Add(pair);
                    }
                }
            }
            RecentItems.CollectionChanged += RecentItems_CollectionChanged;

            String rdjte = localSettings.Values["RecordedDevices"] as String;
            if (!string.IsNullOrWhiteSpace(rdjte))
            {
                if (JsonArray.TryParse(rdjte, out var rdjT))
                {
                    foreach (var jO in rdjT)
                    {
                        RecordedDevices.Add(jO.GetString());
                    }
                }
            }
            RecordedDevices.CollectionChanged += RecordedDevices_CollectionChanged;
        }

        private static void RecordedDevices_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var jArr = new JsonArray();
            foreach (var item in sender as IEnumerable<string>)
            {
                jArr.Add(JsonValue.CreateStringValue(item));
            }
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["RecordedDevices"] = jArr.Stringify();
        }

        public static List<string> PlayingItems { get; set; } = new List<string>();
        public static ObservableCollection<string> RecordedDevices { get; internal set; } = new ObservableCollection<string>();

        internal static void AddToPlayingList(Music music)
        {
            var hash = CurrentServerInstance.DireInfo.Musics.FirstOrDefault(x => x.Value.GetHashCode() == music.GetHashCode());
            if (!hash.Equals(default(KeyValuePair<string,Music>)))
            {
                PlayingItems.Add(hash.Key);
            }
            CurrentHash = hash.Key;
        }

        private static void RecentItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var jArr = new JsonArray();
            foreach (var item in sender as IEnumerable<RecentItemPair>)
            {
                var _title = item.Title;
                var jO = new JsonObject();
                jO["title"] = JsonValue.CreateStringValue(_title);

                jArr.Add(jO);
            }
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["RecentItems"] = jArr.Stringify();
        }
    }
}
