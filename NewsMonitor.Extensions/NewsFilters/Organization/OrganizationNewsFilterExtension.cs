using NewsMonitor.Data.Database;
using NewsMonitor.Data.Models;
using NewsMonitor.WPF.Extensions;
using NewsMonitor.WPF.Settings.Mappings;
using NewsMonitor.WPF.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NewsMonitor.Extensions.NewsFilters.Organization
{
    public class OrganizationNewsFilterExtension : INewsFilterExtension
    {
        public string Name => "Organization";

        ObservableCollection<string> FilteredOrganizations;
        ObservableCollectionSettingsMapping Mapping;

        public OrganizationNewsFilterExtension()
        {
            FilteredOrganizations = new ObservableCollection<string>();
            Mapping = new ObservableCollectionSettingsMapping(
                OrganizationNewsFilterSettingsPage.FilteredOrganizationsKey, null, FilteredOrganizations);
        }

        public bool AllowArticle(NewsArticle newsArticle, string searchTerm, KeyValueStorage storage)
        {
            Mapping.Load(storage);

            return !FilteredOrganizations.Contains(newsArticle.OrganizationName);
        }

        public SettingsPage CreateSettingsPage()
        {
            // modifies FilteredOrganizations if it was updated in the settings
            return new OrganizationNewsFilterSettingsPage(FilteredOrganizations);
        }

        public Window CreateQuickFilterWindow(NewsArticle newsArticle, KeyValueStorage storage)
        {
            Mapping.Load(storage);
            if (!FilteredOrganizations.Contains(newsArticle.OrganizationName))
            {
                FilteredOrganizations.Add(newsArticle.OrganizationName);
                Mapping.Save(storage);
            }

            return null;
        }
    }
}
