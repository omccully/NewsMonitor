using NewsMonitor.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.Services
{
    public class NewsArticlesFoundEventArgs : EventArgs
    {
        public IEnumerable<NewsArticle> NewsArticles { get; private set; }

        public NewsArticlesFoundEventArgs(IEnumerable<NewsArticle> articles)
        {
            this.NewsArticles = articles;
        }
    }
}
