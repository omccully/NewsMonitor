using NewsMonitor.WPF.Extensions;
using NewsMonitor.WPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.WPF.Settings
{
    class GeneralSettingsPageFactory : ISettingsPageFactory
    {
        public SettingsPage CreateSettingsPage()
        {
            return new GeneralSettingsPage();
        }
    }
}
