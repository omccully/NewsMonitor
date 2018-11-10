using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NewsMonitor.Data.Models;

namespace NewsMonitor.Services.NewsSearchers
{ 
    public interface INewsSearcher
    {
        Task<IEnumerable<NewsArticle>> SearchAsync(string term);
    }
}
