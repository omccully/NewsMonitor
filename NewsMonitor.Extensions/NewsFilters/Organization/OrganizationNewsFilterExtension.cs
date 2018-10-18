using NewsMonitor.Data.Database;
using NewsMonitor.Data.Models;
using NewsMonitor.WPF.Extensions;
using NewsMonitor.WPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.Extensions.NewsFilters.Organization
{
    public class OrganizationNewsFilterExtension : INewsFilterExtension
    {
        public string Name => "Organization";

        public bool AllowArticle(KeyValueStorage storage, string searchTerm, NewsArticle newsArticle)
        {
            throw new NotImplementedException();
        }

        public SettingsPage CreateSettingsPage()
        {
            return new OrganizationNewsFilterSettingsPage();
        }
    }
}
