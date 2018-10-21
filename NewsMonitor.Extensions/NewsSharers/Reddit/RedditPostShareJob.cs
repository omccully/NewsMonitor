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

        const int MaxTitleLengthInDescription = 50;

        public RedditPostShareJob(string title, string subreddit, string url, string description = null)
        {
            this.Title = title;
            this.Subreddit = subreddit;
            this.Url = url;

            // TODO: shorten title with elipses
            this.Description = description ?? 
                $"Posting \"{new string(title.Take(MaxTitleLengthInDescription).ToArray())}\" in /r/{subreddit}";
        }

        public async Task Execute()
        {
            for(int i = 20; i > 0; i--)
            {
                await Task.Delay(1000);
                StatusUpdate?.Invoke(this, new ShareJobStatusEventArgs(this, $"Waiting {i} more seconds"));
            }
            Finished?.Invoke(this, new EventArgs());
        }

        public override string ToString()
        {
            return Description;
        }
    }
}
