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
        public static void Initialize()
        {
            ServerInstances = new ObservableCollection<ServerInstance>();
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

            Utils.RemoteUtil.StartFindServers();
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
