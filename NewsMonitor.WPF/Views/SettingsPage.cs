using NewsMonitor.Data.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NewsMonitor.WPF.Views
{
    public abstract class SettingsPage : Page
    {
        public KeyValueStorage KeyValueStorage { get; set; }

        public abstract void Restore();

        public abstract void Save();
    }
}
