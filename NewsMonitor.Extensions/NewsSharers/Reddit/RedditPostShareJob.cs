using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using NewsMonitor.WPF.Extensions;
using NewsMonitor.WPF.Settings;
using RedditSharp.Things;

namespace NewsMonitor.Extensions.NewsSharers.Reddit
{
    [Serializable]
    public class RedditPostShareJob : IShareJob
    {
        public string Description
        {
            get
            {
                // TODO: shorten title with elipses
                return $"Posting \"{new string(Title.Take(MaxTitleLengthInDescription).ToArray())}\" in /r/{Subreddit}";
            }
        }

        public event EventHandler<ShareJobStatusEventArgs> StatusUpdate;
        public event EventHandler Finished;

        public string Title;
        public string Subreddit;
        public string Url;

        const int MaxTitleLengthInDescription = 50;

        [XmlIgnore]
        public IRedditPoster RedditPoster { get; set; }
        //public RedditSharp.Reddit RedditApi { get; set; }

        public RedditPostShareJob()
        {

        }

        public RedditPostShareJob(string title, string subreddit, string url, IRedditPoster redditPoster)
        {
            this.Title = title;
            this.Subreddit = subreddit;
            this.Url = url;
            this.RedditPoster = redditPoster;
        }

        public async Task Execute()
        {
            await RedditPoster.PostUrl(Title, Url, Subreddit);
            Finished?.Invoke(this, new EventArgs());
        }

        public void Skip()
        {
            Finished?.Invoke(this, new EventArgs());
        }

        public override string ToString()
        {
            return Description;
        }
    }
}
