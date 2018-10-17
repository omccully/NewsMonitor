using NewsMonitor.Data.Database;
using NewsMonitor.WPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.WPF.Settings
{
    public interface ISettingsGroup
    {
        string Name { get; }
        IEnumerable<ISettingsGroup> Children { get; }
        KeyValueStorage KeyValueStorage { get; }

        /// <summary>
        /// Injects a KeyValueStorage into the SettingsPage
        /// </summary>
        /// <returns></returns>
        SettingsPage CreateSettingsPageWithStorage();
    }
}
