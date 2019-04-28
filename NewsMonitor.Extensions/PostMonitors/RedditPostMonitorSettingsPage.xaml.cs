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

namespace NewsMonitor.Extensions.PostMonitors
{
    /// <summary>
    /// Interaction logic for RedditNewsSharerSettingsPage.xaml
    /// </summary>
    public partial class RedditPostMonitorSettingsPage : SettingsPage
    {
        public RedditPostMonitorSettingsPage()
        {
            InitializeComponent();

            SettingsMappings.Add(
                new TextBoxSettingsMapping(UpvoteThreshold, null, UpvoteThresholdTextBox));
            SettingsMappings.Add(
                new TextBoxSettingsMapping(CommentTheshold, null, CommentThresholdTextBox));
        }

        public const string UpvoteThreshold = "UpvoteThreshold";
        public const string CommentTheshold = "CommentTheshold";
    }
}
