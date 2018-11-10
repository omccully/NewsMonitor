using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetSharp;

namespace NewsMonitor.Extensions.NewsSharers.Twitter
{
    public class TweetSharpTweeter : ITweeter
    {
        string ConsumerKey, ConsumerSecret, AccessToken, AccessTokenSecret;

        TwitterService service;

        public TweetSharpTweeter(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
        {
            UpdateCredentials(consumerKey, consumerSecret, accessToken, accessTokenSecret);
        }

        public TweetSharpTweeter()
        {

        }

        public void UpdateCredentials(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
        {
            if (ConsumerKey == consumerKey && ConsumerSecret == consumerSecret &&
                AccessToken == accessToken && AccessTokenSecret == accessTokenSecret) return;

            this.ConsumerKey = consumerKey;
            this.ConsumerSecret = consumerSecret;
            this.AccessToken = accessToken;
            this.AccessTokenSecret = accessTokenSecret;

            service = new TwitterService(consumerKey, consumerSecret);
            service.AuthenticateWith(accessToken, accessTokenSecret);
        }

        string TweetSync(string tweet)
        {
            IAsyncResult result = service.BeginSendTweet(
                new SendTweetOptions() { Status = tweet });

            TwitterStatus ts = service.EndSendTweet(result);

            if (ts == null) throw new Exception("Error sending tweet");

            return $"https://twitter.com/{ts.User.ScreenName}/status/{ts.Id}";
        }

        public Task<string> Tweet(string tweet)
        {
            return Task.Run(() => TweetSync(tweet));
        }
    }
}
