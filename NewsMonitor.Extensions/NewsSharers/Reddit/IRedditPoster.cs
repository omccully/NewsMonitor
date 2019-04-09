using NewsMonitor.Extensions.NewsSharers.Reddit.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.Extensions.NewsSharers.Reddit
{
    public interface IRedditPoster
    {
        IRedditCredentials Credentials { get; set; }

        Task<string> PostUrl(string title, string url, string subreddit);
    }
}
