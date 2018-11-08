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
using NewsMonitor.WPF.Settings.Mappings;
using NewsMonitor.WPF.Views.EditableTreeView;

namespace NewsMonitor.Extensions.NewsSharers.Twitter
{
    /// <summary>
    /// Interaction logic for RedditNewsSharerSettingsPage.xaml
    /// </summary>
    public partial class TwitterNewsSharerSettingsPage : SettingsPage
    {
        public TwitterNewsSharerSettingsPage()
        {
            InitializeComponent();

            SettingsMappings.Add(
                new TextBoxSettingsMapping(TwitterConsumerKeyKey, null, TwitterConsumerKey));
            SettingsMappings.Add(
                new PasswordBoxSettingsMapping(TwitterConsumerSecretKey, null, TwitterConsumerSecret));
            SettingsMappings.Add(
                new TextBoxSettingsMapping(TwitterAccessTokenKey, null, TwitterAccessToken));
            SettingsMappings.Add(
                new PasswordBoxSettingsMapping(TwitterAccessTokenSecretKey, null, TwitterAccessTokenSecret));
        }

        public const string TwitterConsumerKeyKey = "TwitterConsumerKey";
        public const string TwitterConsumerSecretKey = "TwitterConsumerSecret";
        public const string TwitterAccessTokenKey = "TwitterAccessToken";
        public const string TwitterAccessTokenSecretKey = "TwitterAccessTokenSecret";
    }
}
