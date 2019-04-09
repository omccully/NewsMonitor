using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsMonitor.Extensions.NewsSharers.Reddit.Common;
using RedditSharp.Things;

namespace NewsMonitor.Extensions.NewsSharers.Reddit
{
    class RedditSharpPoster : RedditSharpClient, IRedditPoster
    {
        public RedditSharpPoster()
        {

        }

        public RedditSharpPoster(IRedditCredentials credentials)
        {
            Credentials = credentials;
        }

        public async Task<string> PostUrl(string title, string url, string subreddit)
        {
            PrepareClient();

            Subreddit sub = await RedditApi.GetSubredditAsync(subreddit);

            try
            {
                Post post = await sub.SubmitPostAsync(title, url);

                return new Uri(new Uri("https://reddit.com/"), post.Permalink).ToString();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("reddit post fail: " + e.ToString());
                throw e;
            }
        }
    }
}
