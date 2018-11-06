using NewsMonitor.Data.Database;
using NewsMonitor.Data.Models;
using NewsMonitor.WPF.Extensions;
using NewsMonitor.WPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NewsMonitor.Extensions.NewsFilters.Recency
{
    public class RecencyNewsFilterExtension : INewsFilterExtension
    {
        public string Name => "Recency";

        public bool AllowArticle(NewsArticle newsArticle, string searchTerm, KeyValueStorage storage)
        {
            int days = storage.GetInteger(RecencyNewsFilterSettingsPage.MaxArticleAgeDaysKey,
                Int32.Parse(RecencyNewsFilterSettingsPage.MaxArticleAgeDefault));

            return (DateTime.Now - newsArticle.TimePublished) < TimeSpan.FromDays(days);
        }

        public Window CreateQuickFilterWindow(NewsArticle newsArticle, KeyValueStorage storage)
        {
            return null;
        }

        public SettingsPage CreateSettingsPage()
        {
            return null;
        }
    }
}
