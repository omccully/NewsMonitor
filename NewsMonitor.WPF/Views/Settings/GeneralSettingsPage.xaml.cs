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
using NewsMonitor.Data.Database;
using NewsMonitor.WPF.Settings;

namespace NewsMonitor.WPF.Views
{
    /// <summary>
    /// Interaction logic for GeneralSettingsPage.xaml
    /// </summary>
    public partial class GeneralSettingsPage : SettingsPage
    {
        public GeneralSettingsPage()
        {
            InitializeComponent();

            SettingsMappings.Add(
                new TextBoxSettingsMapping(SearchTermsKey, SearchTermsDefault, SearchTermsTextBox));
        }

        public const string MaxArticleAgeDaysKey = "MaxArticleAgeDays";
        public const string SearchTermsKey = "SearchTerms";

        const string MaxArticleAgeDefault = "31";
        const string SearchTermsDefault = "";


        /*public override void Restore()
        {
            
            MaxArticleAgeDaysTextBox.Text = KeyValueStorage.GetString(MaxArticleAgeDaysKey, MaxArticleAgeDefault);
            SearchTermsTextBox.Text = KeyValueStorage.GetString(SearchTermsKey, SearchTermsDefault);
        }

        public override void Save()
        {
            // TODO: validation
            KeyValueStorage.SetValue(MaxArticleAgeDaysKey, MaxArticleAgeDaysTextBox.Text);
            KeyValueStorage.SetValue(SearchTermsKey, SearchTermsTextBox.Text);
        }*/
    }
}
