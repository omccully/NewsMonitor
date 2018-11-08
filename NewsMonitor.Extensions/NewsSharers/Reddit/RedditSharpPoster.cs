using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsMonitor.WPF.Settings;
using RedditSharp;
using RedditSharp.Things;

namespace NewsMonitor.Extensions.NewsSharers.Reddit
{
    class RedditSharpPoster : IRedditPoster
    {
        string Username, Password, ClientId, ClientSecret;

        public RedditSharpPoster()
        {

        }

        public RedditSharpPoster(string username, string password,
            string clientId, string clientSecret)
        {
            UpdateCredentials(username, password, clientId, clientSecret);
        }



        public async Task PostUrl(string title, string url, string subreddit)
        {
            if (RedditApi == null)
            {
                throw new InvalidConfigurationException(
                    "You must fill in your Reddit information into the settings page first.");
            }

            Subreddit sub = await RedditApi.GetSubredditAsync(subreddit);

            try
            {
                Post post = await sub.SubmitPostAsync(title, url);

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("reddit post fail: " + e.ToString());
                throw e;
            }
        }

        RedditSharp.Reddit RedditApi;

        public void UpdateCredentials(string username, string password, string clientId, string clientSecret, string userAgent=null)
        {
            if (userAgent == null) userAgent = "News Sharer";
            if (String.IsNullOrWhiteSpace(username) ||
                String.IsNullOrWhiteSpace(password) ||
                String.IsNullOrWhiteSpace(clientId) ||
                String.IsNullOrWhiteSpace(clientSecret)) return;

            if (Username == username && Password == password &&
                ClientId == clientId && ClientSecret == clientSecret)
                return;
            

            this.Username = username;
            this.Password = password;
            this.ClientId = clientId;
            this.ClientSecret = clientSecret;

            BotWebAgent botWebAgent = new BotWebAgent(username, password, clientId, clientSecret,
                 "https://localhost/");
            botWebAgent.UserAgent = $"{userAgent} (/u/{username})";
            RedditApi = new RedditSharp.Reddit(botWebAgent, false);

        }
    }
}
