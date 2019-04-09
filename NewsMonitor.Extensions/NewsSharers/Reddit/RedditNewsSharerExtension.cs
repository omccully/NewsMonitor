using NewsMonitor.Data.Database;
using NewsMonitor.WPF.Extensions;
using NewsMonitor.WPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NewsMonitor.Data.Models;
using System.Xml.Serialization;
using System.IO;
using RedditSharp;
using NewsMonitor.Extensions.NewsSharers.Reddit.Common;

namespace NewsMonitor.Extensions.NewsSharers.Reddit
{
    public class RedditNewsSharerExtension : NewsSharerExtension<RedditPostShareJob>
    {
        public override string Name => "Reddit";

        public RedditNewsSharerExtension()
        {

        }

        public override SettingsPage CreateSettingsPage()
        {
            return new RedditNewsSharerSettingsPage();
        }

        IRedditPoster RedditPoster;
        void InitializeRedditPoster(KeyValueStorage kvs)
        {
            if(RedditPoster == null)
            {
                RedditPoster = new RedditSharpPoster(new RedditSettings(kvs));
            }
        }

        public override NewsSharerWindow CreateSharerWindow(NewsArticle newsArticle, KeyValueStorage kvs)
        {
            InitializeRedditPoster(kvs);

            RedditNewsSharerWindow sharerWindow = new RedditNewsSharerWindow(newsArticle, kvs, RedditPoster);

            base.BeginListenForJobsFromSharerWindow(sharerWindow, kvs);

            return sharerWindow;
        }

        protected override IEnumerable<RedditPostShareJob> DeserializeShareJobs(string serialized, KeyValueStorage kvs)
        {
            IEnumerable<RedditPostShareJob> jobs = base.DeserializeShareJobs(serialized, kvs);

            InitializeRedditPoster(kvs);
            foreach (RedditPostShareJob job in jobs)
            {
                job.RedditPoster = RedditPoster;
            }

            return jobs;
        }
    }
}
