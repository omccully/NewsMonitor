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
        }

        private void PostButton_Click(object sender, RoutedEventArgs e)
        {
            List<IShareJob> jobs = new List<IShareJob>();
            foreach (object item in SelectedSubredditsListView.Items)
            {
                string subreddit = (string)item;
                jobs.Add(new RedditPostShareJob(PostTitleTextBox.Text, subreddit, ArticleUrlTextBox.Text, RedditApi));
            }

            base.OnJobsCreated(new JobsCreatedEventArgs(jobs));
            this.Close();
        }

        private void AddSelectedSubredditsButton_Click(object sender, RoutedEventArgs e)
        {
            //DefaultSubredditsTreeView.Selected
        }

        private void AddSubredditButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedSubredditsListView.Items.Add(SubredditInputTextBox.Text);
        }
    }
}
