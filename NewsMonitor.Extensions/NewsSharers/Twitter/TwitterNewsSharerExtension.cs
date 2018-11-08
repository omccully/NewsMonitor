using NewsMonitor.Data.Database;
using NewsMonitor.Data.Models;
using NewsMonitor.WPF.Extensions;
using NewsMonitor.WPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.Extensions.NewsSharers.Twitter
{
    public class TwitterNewsSharerExtension : NewsSharerExtension<TwitterTweetShareJob>
    {
        public override string Name => "Twitter";

        CredentialUpdatingTweeter Tweeter;

        void InitializeTweeter(KeyValueStorage kvs)
        {
            if(Tweeter == null)
            {
                Tweeter = new CredentialUpdatingTweeter(new TweetSharpTweeter(), kvs);
            }
        }

        public override SettingsPage CreateSettingsPage()
        {
            return new TwitterNewsSharerSettingsPage();
            // update Twitter credentials when settings page is saved...
        }

        public override NewsSharerWindow CreateSharerWindow(NewsArticle newsArticle, KeyValueStorage kvs)
        {
            InitializeTweeter(kvs);
            TwitterNewsSharerWindow window = new TwitterNewsSharerWindow(newsArticle, kvs, Tweeter);
            base.BeginListenForJobsFromSharerWindow(window, kvs);
            return window;
        }

        protected override IEnumerable<TwitterTweetShareJob> DeserializeShareJobs(string serialized, KeyValueStorage kvs)
        {
            IEnumerable<TwitterTweetShareJob> shareJobs =  base.DeserializeShareJobs(serialized, kvs);

            InitializeTweeter(kvs);
            foreach (TwitterTweetShareJob job in shareJobs)
            {
                // inject into job
                job.Tweeter = this.Tweeter;
            }

            return shareJobs;
        }
    }
}
