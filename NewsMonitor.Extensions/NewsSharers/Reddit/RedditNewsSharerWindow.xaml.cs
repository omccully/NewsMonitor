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
using System.Windows.Shapes;
using NewsMonitor.Data.Database;
using NewsMonitor.Data.Models;
using NewsMonitor.WPF.Views;
using NewsMonitor.WPF.Extensions;
using NewsMonitor.WPF.Views.EditableTreeView;
using NewsMonitor.WPF.Settings.Mappings;
using System.Collections.ObjectModel;

namespace NewsMonitor.Extensions.NewsSharers.Reddit
{
    /// <summary>
    /// Interaction logic for RedditNewsSharerWindow.xaml
    /// </summary>
    public partial class RedditNewsSharerWindow : NewsSharerWindow
    {
        //NewsArticle newsArticle
        RedditSharp.Reddit RedditApi;

        public RedditNewsSharerWindow(NewsArticle newsArticle, KeyValueStorage kvs, RedditSharp.Reddit reddit)
        {
            InitializeComponent();

            PostTitleTextBox.Text = newsArticle.Title;
            ArticleUrlTextBox.Text = newsArticle.Url;
            this.RedditApi = reddit;

            TreeViewSettingsMapping map = new TreeViewSettingsMapping(
                RedditNewsSharerSettingsPage.RedditDefaultSubredditsKey, null, DefaultSubredditsTreeView);
            map.Deserialize(kvs.GetString(RedditNewsSharerSettingsPage.RedditDefaultSubredditsKey));
            DefaultSubredditsTreeView.ExpandAll();
            //DefaultSubredditsTreeView.RestoreFromTreeModel()

            SelectedSubreddits = new ObservableCollection<string>();
            SelectedSubredditsListView.ItemsSource = SelectedSubreddits;
            SelectedSubredditsListView.KeyUp += SelectedSubredditsListView_KeyUp;
        }

        private void SelectedSubredditsListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Delete) return;
            IEnumerable<string> selectedSubreddits = SelectedSubredditsListView.SelectedItems.Cast<string>().ToList();
            foreach (string selectedSubreddit in selectedSubreddits)
            {
                SelectedSubreddits.Remove(selectedSubreddit);
            }
        }

        private void PostButton_Click(object sender, RoutedEventArgs e)
        {
            List<IShareJob> jobs = new List<IShareJob>();
            foreach (string subreddit in SelectedSubreddits)
            {
                jobs.Add(new RedditPostShareJob(PostTitleTextBox.Text, subreddit, ArticleUrlTextBox.Text, RedditApi));
            }

            base.OnJobsCreated(new JobsCreatedEventArgs(jobs));
            this.Close();
        }

        ObservableCollection<string> SelectedSubreddits { get; set; }

        private void AddSelectedSubredditsButton_Click(object sender, RoutedEventArgs e)
        {
            HashSet<string> existingSubreddits = new HashSet<string>(SelectedSubreddits);
            
            foreach(string subreddit in SelectedDefaultSubreddits)
            {
                if (existingSubreddits.Contains(subreddit)) continue;
                SelectedSubreddits.Add(subreddit);
            }
        }

       /* IEnumerable<string> SelectedSubreddits
        {
            get
            {
                return SelectedSubredditsListView.Items.Cast<ListViewItem>()
                    .Select(lvi => lvi.ToString());
            }
        }*/

        IEnumerable<string> SelectedDefaultSubreddits
        {
            get
            {
                return DefaultSubredditsTreeView.Items.Cast<TreeViewItem>()
                    .SelectMany(sectionItem => sectionItem.Items.Cast<TreeViewItem>())
                    .Where(tvi => tvi.IsSelected)
                    .Select(tvi => tvi.Header.ToString());
            }
        }

        private void AddSubredditButton_Click(object sender, RoutedEventArgs e)
        {
            if (!SelectedSubreddits.Contains(SubredditInputTextBox.Text))
            {
                SelectedSubreddits.Add(SubredditInputTextBox.Text);
            }
        }
    }
}
