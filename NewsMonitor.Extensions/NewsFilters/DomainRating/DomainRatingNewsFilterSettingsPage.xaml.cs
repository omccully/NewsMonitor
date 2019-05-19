using NewsMonitor.WPF.Views;
using System;
using System.Collections.Generic;
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
using NewsMonitor.WPF.Settings.Mappings;
using NewsMonitor.WPF.Views.EditableTreeView;
using Microsoft.Win32;
using System.IO;
using NewsMonitor.WPF.Settings;

namespace NewsMonitor.Extensions.NewsFilters.DomainRating
{
    /// <summary>
    /// Interaction logic for DomainRatingNewsFilterSettingsPage.xaml
    /// </summary>
    public partial class DomainRatingNewsFilterSettingsPage : SettingsPage
    {
        public const string DomainRatingsKey = "DomainRatings";

        public DomainRatingNewsFilterSettingsPage(IDomainRatingsSerializer serializer)
        {
            InitializeComponent();

            SettingsMappings.Add(new DataGridSettingsMapping(DomainRatingsKey, 
                null, RatingsDataGrid, serializer));
        }
    }
}
