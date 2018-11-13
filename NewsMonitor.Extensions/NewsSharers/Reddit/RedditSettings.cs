using NewsMonitor.Data.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.Extensions.NewsSharers.Reddit
{
    class RedditSettings
    {
        KeyValueStorage Storage;
    
        public string Username
        {
            get
            {
                return Storage.GetString(RedditNewsSharerSettingsPage.RedditUsernameKey);
            }
            set
            {
                Storage.SetValue(RedditNewsSharerSettingsPage.RedditUsernameKey, value);
            }
        }

        public string Password
        {
            get
            {
                return Storage.GetString(RedditNewsSharerSettingsPage.RedditPasswordKey);
            }
            set
            {
                Storage.SetValue(RedditNewsSharerSettingsPage.RedditPasswordKey, value);
            }
        }

        public string ClientId
        {
            get
            {
                return Storage.GetString(RedditNewsSharerSettingsPage.RedditClientIdKey);
            }
            set
            {
                Storage.SetValue(RedditNewsSharerSettingsPage.RedditClientIdKey, value);
            }
        }

        public string ClientSecret
        {
            get
            {
                return Storage.GetString(RedditNewsSharerSettingsPage.RedditClientSecretKey);
            }
            set
            {
                Storage.SetValue(RedditNewsSharerSettingsPage.RedditClientSecretKey, value);
            }
        }

        public string UserAgent
        {
            get
            {
                return Storage.GetString(RedditNewsSharerSettingsPage.RedditUserAgentKey);
            }
            set
            {
                Storage.SetValue(RedditNewsSharerSettingsPage.RedditUserAgentKey, value);
            }
        }

        public RedditSettings(KeyValueStorage kvs)
        {
            this.Storage = kvs;
        }
    }
}
