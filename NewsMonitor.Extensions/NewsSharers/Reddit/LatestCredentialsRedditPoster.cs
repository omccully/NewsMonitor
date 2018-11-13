using NewsMonitor.Data.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.Extensions.NewsSharers.Reddit
{
    class LatestCredentialsRedditPoster : IRedditPoster
    {
        IRedditPoster InnerPoster;
        RedditSettings Settings;

        public LatestCredentialsRedditPoster(IRedditPoster innerPoster, RedditSettings settings)
        {
            this.InnerPoster = innerPoster;
            this.Settings = settings;
        }

        public Task<string> PostUrl(string title, string url, string subreddit)
        {
            InnerPoster.UpdateCredentials(Settings.Username, Settings.Password, 
                Settings.ClientId, Settings.ClientSecret, Settings.UserAgent);
            return InnerPoster.PostUrl(title, url, subreddit);
        }

        public void UpdateCredentials(string username, string password, string clientId, string clientSecret, string userAgent = null)
        {
            Settings.Username = username;
            Settings.Password = password;
            Settings.ClientId = clientId;
            Settings.ClientSecret = clientSecret;
            Settings.UserAgent = userAgent;
        }
    }
}
