using System;
using System.Collections.Generic;
using System.Text;
using NewsMonitor.Data.Models;

namespace NewsMonitor.Services.NewsSearchers
{ 
    public interface INewsSearcher
    {
        IEnumerable<NewsArticle> Search(string term);
    }
}
