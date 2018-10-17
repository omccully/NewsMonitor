using NewsMonitor.Services.NewsSearchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using NewsMonitor.Data.Database;
using NewsMonitor.WPF.Views;

namespace NewsMonitor.WPF.Extensions
{
    public interface INewsSearcherExtension : ISettingsGroupExtension
    {
        INewsSearcher CreateNewsSearcher(KeyValueStorage kvs);
    }
}
