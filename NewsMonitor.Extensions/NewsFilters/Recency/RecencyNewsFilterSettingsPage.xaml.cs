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
using NewsMonitor.WPF.Settings;

namespace NewsMonitor.Extensions.NewsFilters.Recency
{
    /// <summary>
    /// Interaction logic for RecencyNewsFilterSettingsPage.xaml
    /// </summary>
    public partial class RecencyNewsFilterSettingsPage : SettingsPage
    {
        public RecencyNewsFilterSettingsPage()
        {
            InitializeComponent();

            SettingsMappings.Add(
                new TextBoxSettingsMapping(MaxArticleAgeDaysKey,
                    MaxArticleAgeDefault, MaxArticleAgeDaysTextBox,
                    TextValidator.IntegerValidator));
        }

        public const string MaxArticleAgeDaysKey = "MaxArticleAgeDays";
        public const string MaxArticleAgeDefault = "31";
    }
}
