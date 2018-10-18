using NewsMonitor.Data.Database;
using NewsMonitor.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.WPF.Extensions
{
    public interface INewsFilterExtension : ISettingsGroupExtension
    {
        bool AllowArticle(KeyValueStorage storage, string searchTerm, NewsArticle newsArticle);
    }
}
