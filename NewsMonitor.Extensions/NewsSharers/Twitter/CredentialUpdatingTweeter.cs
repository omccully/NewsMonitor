﻿using NewsMonitor.Data.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.Extensions.NewsSharers.Twitter
{
    public class CredentialUpdatingTweeter : ITweeter
    {
        ITweeter InnerTweeter;
        KeyValueStorage Storage;

        public CredentialUpdatingTweeter(ITweeter innerTweeter, KeyValueStorage storage)
        {
            this.InnerTweeter = innerTweeter;
            this.Storage = storage;
        }

        public async Task Tweet(string tweet)
        {
            string TwitterConsumerKey = Storage.GetString(TwitterNewsSharerSettingsPage.TwitterConsumerKeyKey);
            string TwitterConsumerSecret = Storage.GetString(TwitterNewsSharerSettingsPage.TwitterConsumerSecretKey);

            string TwitterAccessToken = Storage.GetString(TwitterNewsSharerSettingsPage.TwitterAccessTokenKey);
            string TwitterAccessTokenSecret = Storage.GetString(TwitterNewsSharerSettingsPage.TwitterAccessTokenSecretKey);
            InnerTweeter.UpdateCredentials(TwitterConsumerKey, TwitterConsumerSecret, TwitterAccessToken, TwitterAccessTokenSecret);

            await InnerTweeter.Tweet(tweet);
        }

        public void UpdateCredentials(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
        {
            Storage.SetValue(TwitterNewsSharerSettingsPage.TwitterConsumerKeyKey, consumerKey);
            Storage.SetValue(TwitterNewsSharerSettingsPage.TwitterConsumerSecretKey, consumerSecret);
            Storage.SetValue(TwitterNewsSharerSettingsPage.TwitterAccessTokenKey, accessToken);
            Storage.SetValue(TwitterNewsSharerSettingsPage.TwitterAccessTokenSecretKey, accessTokenSecret);
        }
    }
}
