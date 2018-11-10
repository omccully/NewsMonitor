using NewsMonitor.WPF.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NewsMonitor.Extensions.NewsSharers.Twitter
{
    [Serializable]
    public class TwitterTweetShareJob : IShareJob
    {
        public string Description => Comment;
        public const int MaxTweetLength = 280;
        public const int TweetUrlCharactersUsed = 24;
        public const int MaxTweetLengthWithUrl = MaxTweetLength - TweetUrlCharactersUsed;

        public string Url;
        public string Comment;


        [XmlIgnore]
        public ITweeter Tweeter { get; set; }

        public TwitterTweetShareJob(string comment, string url, ITweeter tweeter)
        {
            if (comment.Length > MaxTweetLengthWithUrl)
                throw new ArgumentException("Tweet description is too long. The max length is " + MaxTweetLengthWithUrl);
            this.Comment = comment;
            this.Url = url;
            this.Tweeter = tweeter;
        }

        public TwitterTweetShareJob()
        {
                
        }

        public event EventHandler<ShareJobStatusEventArgs> StatusUpdate;
        public event EventHandler<ShareJobFinishedEventArgs> Finished;

        public async Task Execute()
        {
            string tweetUrl = await Tweeter.Tweet(Description + " " + Url);
            Finished?.Invoke(this, new ShareJobFinishedEventArgs(tweetUrl));
        }

        public void Skip()
        {
            Finished?.Invoke(this, ShareJobFinishedEventArgs.Skipped);
        }
    }
}
