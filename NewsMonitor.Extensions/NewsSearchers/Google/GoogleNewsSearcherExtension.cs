using NewsMonitor.Data.Database;
using NewsMonitor.Services.NewsSearchers;
using NewsMonitor.WPF.Extensions;
using NewsMonitor.WPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.Extensions.NewsSearchers.Google
{
    public class GoogleNewsSearcherExtension : INewsSearcherExtension
    {
        public string Name => "Google";

        public INewsSearcher CreateNewsSearcher(KeyValueStorage kvs)
        {
            return new GoogleNewsSearcher();
        }

        public SettingsPage CreateSettingsPage()
        {
            return null;
        }
    }
}
