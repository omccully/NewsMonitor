using NewsMonitor.Extensions.NewsSharers.Reddit.Common;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.Extensions.PostMonitors
{
    class RedditSharpReader : RedditSharpClient
    {
        public RedditSharpReader()
        {

        }

        public RedditSharpReader(IRedditCredentials credentials)
        {
            Credentials = credentials;
        }

        public async Task<Post> GetPostInfo(string url)
        {
            PrepareClient();

            Post post = await RedditApi.GetPostAsync(new Uri(url));

            return post;
        }
    }
}
