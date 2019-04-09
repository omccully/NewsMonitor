using NewsMonitor.Extensions.NewsSharers.Reddit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.Extensions.NewsSharers.Reddit.Common
{
    class LatestRedditCredentials : IRedditCredentials
    {
        public string Username
        {
            get
            {
                return _settings.Username;
            }
            set
            {
                _settings.Username = value;
            }
        }

        public string Password
        {
            get
            {
                return _settings.Password;
            }
            set
            {
                _settings.Password = value;
            }
        }

        public string ClientId
        {
            get
            {
                return _settings.ClientId;
            }
            set
            {
                _settings.ClientId = value;
            }
        }

        public string ClientSecret
        {
            get
            {
                return _settings.ClientSecret;
            }
            set
            {
                _settings.ClientSecret = value;
            }
        }

        public string UserAgent
        {
            get
            {
                return _settings.UserAgent;
            }
            set
            {
                _settings.UserAgent = value;
            }
        }

        RedditSettings _settings;
        public LatestRedditCredentials(RedditSettings settings)
        {
            _settings = settings;
        }
    }
}
