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

namespace NewsMonitor.Extensions.NewsSharers.Reddit
{
    /// <summary>
    /// Interaction logic for RedditNewsSharerSettingsPage.xaml
    /// </summary>
    public partial class RedditNewsSharerSettingsPage : SettingsPage
    {
        IEnumerable<EditableTreeViewLevelRule> EditableTreeViewRules = new List<EditableTreeViewLevelRule>()
        {
            new EditableTreeViewLevelRule("<section>", true),
            new EditableTreeViewLevelRule("<subreddit>", true)
        };

        public RedditNewsSharerSettingsPage()
        {
            InitializeComponent();

            SettingsMappings.Add(
                new TextBoxSettingsMapping(RedditUsernameKey, null, RedditUsernameTextBox));
            SettingsMappings.Add(
                new PasswordBoxSettingsMapping(RedditPasswordKey, null, RedditPasswordTextBox));
            SettingsMappings.Add(
                new TextBoxSettingsMapping(RedditClientIdKey, null, RedditClientIdTextBox));
            SettingsMappings.Add(
                new PasswordBoxSettingsMapping(RedditClientSecretKey, null, RedditClientSecretTextBox));
            SettingsMappings.Add(
                new EditableTreeViewSettingsMapping(RedditDefaultSubredditsKey, null, SubredditOptionsTreeView, EditableTreeViewRules));
        }

        public const string RedditUsernameKey = "RedditUsername";
        public const string RedditPasswordKey = "RedditPassword";
        public const string RedditClientIdKey = "RedditClientId";
        public const string RedditClientSecretKey = "RedditClientSecret";
        public const string RedditUserAgentKey = "RedditUserAgent";
        public const string RedditDefaultSubredditsKey = "DefaultSubreddits";

        const string SectionText = "<section>";
        const string SubredditText = "<subreddit>";
    }
}
