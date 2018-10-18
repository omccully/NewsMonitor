using NewsMonitor.Data.Database;
using NewsMonitor.WPF.Extensions;
using NewsMonitor.WPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NewsMonitor.Extensions.NewsSharers.Reddit
{
    public class RedditNewsSharerExtension : INewsSharerExtension
    {
        public string Name => "Reddit";

        public SettingsPage CreateSettingsPage()
        {
            return new RedditNewsSharerSettingsPage();
        }

        public Window CreateSharerWindow(KeyValueStorage kvs)
        {
            throw new NotImplementedException();
        }
    }
}
