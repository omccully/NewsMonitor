using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.Extensions.NewsSharers.Twitter
{
    public interface ITweeter
    {
        void UpdateCredentials(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret);

        Task Tweet(string tweet);
    }
}
