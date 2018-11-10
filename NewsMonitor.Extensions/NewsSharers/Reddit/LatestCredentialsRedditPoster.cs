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
        KeyValueStorage Storage;

        public LatestCredentialsRedditPoster(IRedditPoster innerPoster, KeyValueStorage storage)
        {
            this.InnerPoster = innerPoster;
            this.Storage = storage;
        }

        public Task<string> PostUrl(string title, string url, string subreddit)
        {
            string username = Storage.GetString(RedditNewsSharerSettingsPage.RedditUsernameKey);
            string password = Storage.GetString(RedditNewsSharerSettingsPage.RedditPasswordKey);
            string clientId = Storage.GetString(RedditNewsSharerSettingsPage.RedditClientIdKey);
            string clientSecret = Storage.GetString(RedditNewsSharerSettingsPage.RedditClientSecretKey);
            string userAgent = Storage.GetString(RedditNewsSharerSettingsPage.RedditUserAgentKey);
            InnerPoster.UpdateCredentials(username, password, clientId, clientSecret);
            return InnerPoster.PostUrl(title, url, subreddit);
        }

        public void UpdateCredentials(string username, string password, string clientId, string clientSecret, string userAgent = null)
        {
            Storage.SetValue(RedditNewsSharerSettingsPage.RedditUsernameKey, username);
            Storage.SetValue(RedditNewsSharerSettingsPage.RedditPasswordKey, password);
            Storage.SetValue(RedditNewsSharerSettingsPage.RedditClientIdKey, clientId);
            Storage.SetValue(RedditNewsSharerSettingsPage.RedditClientSecretKey, clientSecret);
            Storage.SetValue(RedditNewsSharerSettingsPage.RedditUserAgentKey, userAgent);
        }
    }
}
