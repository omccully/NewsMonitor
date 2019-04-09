using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.Extensions.NewsSharers.Reddit.Common
{
    public interface IRedditCredentials
    {
        string Username { get; set; }
        string Password { get; set; }
        string ClientId { get; set; }
        string ClientSecret { get; set; }
        string UserAgent { get; set; }
    }

    public static class RedditCredentialsExtensions
    {
        public static bool AreIncomplete(this IRedditCredentials creds)
        {
            return String.IsNullOrWhiteSpace(creds.Username) ||
                String.IsNullOrWhiteSpace(creds.Password) ||
                String.IsNullOrWhiteSpace(creds.ClientId) ||
                String.IsNullOrWhiteSpace(creds.ClientSecret);
        }
    }

}
