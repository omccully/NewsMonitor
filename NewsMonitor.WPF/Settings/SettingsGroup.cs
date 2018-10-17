using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsMonitor.Data.Database;
using NewsMonitor.WPF.Views;
using NewsMonitor.WPF.Extensions;

namespace NewsMonitor.WPF.Settings
{
    public class SettingsGroup : ISettingsGroup
    {
        public string Name { get; private set; }

        public IEnumerable<ISettingsGroup> Children { get; private set; }

        ISettingsPageFactory SettingsPageFactory;
        public KeyValueStorage KeyValueStorage { get; private set; }

        public SettingsGroup(string name, ISettingsPageFactory settingsPageFactory,
            KeyValueStorage keyValueStorage,
            IEnumerable<ISettingsGroup> children=null)
        {
            this.Name = name;
            this.SettingsPageFactory = settingsPageFactory;
            this.KeyValueStorage = keyValueStorage;
            this.Children = children;
        }

        public SettingsGroup(string name, IEnumerable<ISettingsGroup> children)
        {
            this.Name = name;
            this.SettingsPageFactory = null;
            this.KeyValueStorage = null;
            this.Children = children;
        }

        public SettingsPage CreateSettingsPageWithStorage()
        {
            if (SettingsPageFactory == null) return null;

            SettingsPage settingsPage = SettingsPageFactory.CreateSettingsPage();
            if(settingsPage.KeyValueStorage == null)
            {
                settingsPage.KeyValueStorage = KeyValueStorage;
            }
            return settingsPage;
        }
    }
}
