using NewsMonitor.WPF.Settings;
using RedditSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.Extensions.NewsSharers.Reddit.Common
{
    class RedditSharpClient
    {
        public IRedditCredentials Credentials { get; set; }

        RedditCredentials _previouslyUsedCredentials;

        protected RedditSharp.Reddit RedditApi;

        public RedditSharpClient()
        {

        }

        public RedditSharpClient(IRedditCredentials credentials)
        {
            Credentials = credentials;
        }

        protected void PrepareClient()
        {
            if (_previouslyUsedCredentials == null || !Credentials.Equals(_previouslyUsedCredentials))
            {
                // make a copy
                _previouslyUsedCredentials = new RedditCredentials(Credentials);
                InitializeRedditApi(_previouslyUsedCredentials);
            }
        }

        void InitializeRedditApi(IRedditCredentials creds)
        {
            if (creds.AreIncomplete())
            {
                throw new InvalidConfigurationException(
                    "You must fill in your Reddit information into the settings page first.");
            }

            string userAgent = creds.UserAgent ?? "News Sharer";
            BotWebAgent botWebAgent = new BotWebAgent(creds.Username,
                creds.Password, creds.ClientId, creds.ClientSecret,
                 "https://localhost/");
            botWebAgent.UserAgent = $"{userAgent} (/u/{creds.Username})";
            RedditApi = new RedditSharp.Reddit(botWebAgent, false);
        }
    }
}
