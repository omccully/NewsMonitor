using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using NewsMonitor.Data.Database;
using NewsMonitor.Data.Models;
using NewsMonitor.WPF.Views;

namespace NewsMonitor.WPF.Extensions
{
    public interface INewsSharerExtension : ISettingsGroupExtension
    {
        NewsSharerWindow CreateSharerWindow(NewsArticle newsArticle, KeyValueStorage kvs);
        IEnumerable<ISharerJob> GetUnfinishedJobs(KeyValueStorage kvs);
    }
}
