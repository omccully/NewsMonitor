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

namespace NewsMonitor.Extensions.NewsSharers.Twitter
{
    /// <summary>
    /// Interaction logic for RedditNewsSharerWindow.xaml
    /// </summary>
    public partial class TwitterNewsSharerWindow : NewsSharerWindow
    {
        ITweeter Tweeter;

        public TwitterNewsSharerWindow(NewsArticle newsArticle, KeyValueStorage kvs, ITweeter tweeter)
        {
            InitializeComponent();

            this.Tweeter = tweeter;
                
            CommentTextBox.MaxLength = TwitterTweetShareJob.MaxTweetLengthWithUrl;
            CommentTextBox.Text = newsArticle.Title;
            UrlTextBox.Text = newsArticle.Url;

            this.SizeToContent = SizeToContent.Width;
        }


        private void PostButton_Click(object sender, RoutedEventArgs e)
        {
            TwitterTweetShareJob job = new TwitterTweetShareJob(CommentTextBox.Text, UrlTextBox.Text, Tweeter);
            var jobs = new List<IShareJob>() { job };

            base.OnJobsCreated(new JobsCreatedEventArgs(jobs));
            this.Close();
        }
    }
}
