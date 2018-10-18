using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.Extensions.NewsSearchers.Bing
{
    public class InvalidBingNewsApiKeyException : Exception
    {
        public InvalidBingNewsApiKeyException() :
            base("The access key for the Bing News API is invalid.")
        {

        }
    }
}
