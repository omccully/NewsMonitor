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

namespace NewsMonitor.Extensions.NewsSharers.Reddit
{
    public class RedditNewsSharerExtension : INewsSharerExtension
    {
        public string Name => "Reddit";

        public RedditNewsSharerExtension()
        {

        }

        public SettingsPage CreateSettingsPage()
        {
            return new RedditNewsSharerSettingsPage();
        }

        public NewsSharerWindow CreateSharerWindow(NewsArticle newsArticle, KeyValueStorage kvs)
        {
             /*BotWebAgent bwa = new BotWebAgent(
                 kvs.GetString(RedditNewsSharerSettingsPage.RedditUsernameKey),
                 kvs.GetString(RedditNewsSharerSettingsPage.RedditPasswordKey),
                 kvs.GetString(RedditNewsSharerSettingsPage.RedditClientIdKey),
                 kvs.GetString(RedditNewsSharerSettingsPage.RedditClientSecretKey),
                 "https://localhost/");
                 
            RedditSharp.Reddit reddit = new RedditSharp.Reddit(bwa, false);*/
             //reddit.GetSubreddit("/r/news").SubmitPostAsync()


            RedditNewsSharerWindow sharerWindow = new RedditNewsSharerWindow(newsArticle, kvs /*, reddit*/);

            sharerWindow.JobsCreated += delegate (object window, JobsCreatedEventArgs e)
            {
                foreach(IShareJob job in e.Jobs)
                {
                    job.Finished += (jobbbbb, ea) =>
                    {
                        // TODO: remove job from UnfinishedJobs, update kvs
                    };
                }

                // TODO: save unfinished jobs to KeyValueStorage

            };

            return sharerWindow;
        }

        List<IShareJob> UnfinishedJobs = new List<IShareJob>()
        {
            new RedditPostShareJob("test initial", "worldnews", "https://twitter.com")
        };
        public IEnumerable<IShareJob> GetUnfinishedJobs(KeyValueStorage kvs)
        {
            // TODO: deserialize value from kvs and set UnfinishedJobs 


            return UnfinishedJobs;
        }
    }
}
