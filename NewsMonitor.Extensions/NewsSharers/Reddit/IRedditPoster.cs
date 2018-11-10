using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.Extensions.NewsSharers.Reddit
{
    public interface IRedditPoster
    {
        void UpdateCredentials(string username, string password,
            string clientId, string clientSecret, string userAgent = null);

        Task<string> PostUrl(string title, string url, string subreddit);
    }
}
