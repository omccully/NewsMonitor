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
        public RedditSharp.Reddit RedditApi { get; set; }

        public RedditPostShareJob()
        {

        }

        public RedditPostShareJob(string title, string subreddit, string url, RedditSharp.Reddit reddit)
        {
            this.Title = title;
            this.Subreddit = subreddit;
            this.Url = url;
            this.RedditApi = reddit;
        }

        public async Task Execute()
        {
            if(RedditApi == null)
            {
                throw new InvalidConfigurationException(
                    "You must fill in your Reddit information into the settings page first.");
            }

            Subreddit sub = await RedditApi.GetSubredditAsync(Subreddit);

            System.Diagnostics.Debug.WriteLine($"{sub}.SubmitPostAsync({Title}, {Url})");
            try
            {
                Post post = await sub.SubmitPostAsync(Title, Url);
               
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("reddit post fail: " + e.ToString());
            }

            Finished?.Invoke(this, new EventArgs());
        }

        public override string ToString()
        {
            return Description;
        }
    }
}
