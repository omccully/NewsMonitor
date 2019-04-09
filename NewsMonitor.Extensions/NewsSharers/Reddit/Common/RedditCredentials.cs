using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.Extensions.NewsSharers.Reddit.Common
{
    public class RedditCredentials : IRedditCredentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string UserAgent { get; set; }

        public RedditCredentials(IRedditCredentials otherCreds)
        {
            Username = otherCreds.Username;
            Password = otherCreds.Password;
            ClientId = otherCreds.ClientId;
            ClientSecret = otherCreds.ClientSecret;
            UserAgent = otherCreds.UserAgent;
        }

        public override bool Equals(object obj)
        {
            IRedditCredentials otherCreds = obj as IRedditCredentials;
            if (obj == null) return false;

            return Username == otherCreds.Username &&
                Password == otherCreds.Password &&
                ClientId == otherCreds.ClientId &&
                ClientSecret == otherCreds.ClientSecret &&
                UserAgent == otherCreds.UserAgent;
        }

        public override int GetHashCode()
        {
            return Username.GetHashCode() ^
                Password.GetHashCode() ^
                ClientId.GetHashCode() ^
                ClientSecret.GetHashCode() ^
                UserAgent.GetHashCode();
        }
    }
}
