using NewsMonitor.Data.Database;
using NewsMonitor.WPF.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.WPF.Extensions
{
    public class ExtensionFeature<T> where T : ISettingsGroupExtension
    {
        public T Extension { get; private set; }
        public ISettingsGroup SettingsGroup { get; private set; }

        public ExtensionFeature(T extension, ISettingsGroup settingsGroup)
        {
            this.Extension = extension;
            this.SettingsGroup = settingsGroup;
        }

        public string Name
        {
            get
            {
                return Extension.Name;
            }
        }

        public KeyValueStorage KeyValueStorage
        {
            get
            {
                return SettingsGroup.KeyValueStorage;
            }
        }
    }
}
