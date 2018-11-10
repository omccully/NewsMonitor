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
    public class RedditPostShareJob : ShareJob
    {
        public override string Description
        {
            get
            {
                // TODO: shorten title with elipses
                return $"Posting \"{new string(Title.Take(MaxTitleLengthInDescription).ToArray())}\" in /r/{Subreddit}";
            }
        }

        public string Title;
        public string Subreddit;
        public string Url;

        const int MaxTitleLengthInDescription = 50;

        [XmlIgnore]
        public IRedditPoster RedditPoster { get; set; }

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

        protected override Task<string> InnerExecute()
        {
            return RedditPoster.PostUrl(Title, Url, Subreddit);
        }
    }
}
