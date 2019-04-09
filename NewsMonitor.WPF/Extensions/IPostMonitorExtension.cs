using NewsMonitor.Data.Database;
using NewsMonitor.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.WPF.Extensions
{
    public interface IPostMonitorExtension : ISettingsGroupExtension
    {
        event EventHandler<NeedsAttentionEventArgs> NeedsAttention;

        string Domain { get; }

        TimeSpan TimeSpan { get; }

        string ShareSettingsWith { get; }

        Task Monitor(IEnumerable<ShareJobResult> results, KeyValueStorage kvs);
    }
}
