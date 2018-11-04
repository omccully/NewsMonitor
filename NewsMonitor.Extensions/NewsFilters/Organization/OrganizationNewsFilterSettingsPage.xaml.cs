using NewsMonitor.WPF.Settings;
using NewsMonitor.WPF.Settings.Mappings;
using NewsMonitor.WPF.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NewsMonitor.Extensions.NewsFilters.Organization
{
    /// <summary>
    /// Interaction logic for OrganizationNewsFilterSettingsPage.xaml
    /// </summary>
    public partial class OrganizationNewsFilterSettingsPage : SettingsPage
    {
        ObservableCollection<string> FilteredOrganizations;

        public OrganizationNewsFilterSettingsPage()
        {
            InitializeComponent();

            FilteredOrganizations = new ObservableCollection<string>();

            SettingsMappings.Add(
                new ObservableCollectionSettingsMapping(FilteredOrganizationsKey, null, FilteredOrganizations));

            OrganizationListBox.ItemsSource = FilteredOrganizations;
            OrganizationListBox.KeyUp += OrganizationListBox_KeyUp;
        }

        private void OrganizationListBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Delete) return;
            IEnumerable<string> selectedSubreddits = OrganizationListBox.SelectedItems.Cast<string>().ToList();
            foreach (string selectedSubreddit in selectedSubreddits)
            {
                FilteredOrganizations.Remove(selectedSubreddit);
            }
        }

        public const string FilteredOrganizationsKey = "FilteredOrganizations";

        private void NewOrganizationButton_Click(object sender, RoutedEventArgs e)
        {
            if (!FilteredOrganizations.Contains(NewOrganizationTextBox.Text))
            {
                FilteredOrganizations.Add(NewOrganizationTextBox.Text);
                NewOrganizationTextBox.Text = "";
            } 
        }

    }
}
