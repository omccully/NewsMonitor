using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsMonitor.WPF.Extensions;

namespace NewsMonitor.Extensions.NewsSharers.Reddit
{
    public class RedditPostShareJob : IShareJob
    {
        public string Description { get; private set; }

        public event EventHandler<ShareJobStatusEventArgs> StatusUpdate;
        public event EventHandler Finished;

        string Title;
        string Subreddit;
        string Url;

        public RedditPostShareJob(string title, string subreddit, string url, string description)
        {
            this.Title = title;
            this.Subreddit = subreddit;
            this.Url = url;
            this.Description = description;
        }

        public Task Execute()
        {
            return null;
        }
    }
}
