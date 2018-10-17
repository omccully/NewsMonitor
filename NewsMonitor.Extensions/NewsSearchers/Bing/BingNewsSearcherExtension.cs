using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsMonitor.Data.Database;
using NewsMonitor.Services.NewsSearchers;
using NewsMonitor.WPF.Extensions;
using NewsMonitor.WPF.Views;

namespace NewsMonitor.Extensions.NewsSearchers.Bing
{
    public class BingNewsSearcherExtension : INewsSearcherExtension
    {
        public string Name => "Bing";

        public INewsSearcher CreateNewsSearcher(KeyValueStorage kvs)
        {
            string access_key = kvs.GetString(BingNewsSearcherSettingsPage.BingNewsAccessKeyStorageKey);
            if (access_key == null) throw new InvalidOperationException(
                 "The Bing News access key is not properly set");

            return new BingNewsSearcher(access_key);
        }

        public SettingsPage CreateSettingsPage()
        {
            // return new BingNewsSearcherSettingsPage();
            return new BingNewsSearcherSettingsPage();
        }
    }
}
