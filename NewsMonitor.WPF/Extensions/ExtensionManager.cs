using NewsMonitor.Data.Database;
using NewsMonitor.WPF.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.WPF.Extensions
{
    public class ExtensionManager<T> where T : ISettingsGroupExtension
    {
        public string PluralName { get; private set; }
        public IEnumerable<ExtensionFeature<T>> Features { get; private set; }
        public ISettingsGroup SettingsGroup { get; private set; }

        public ExtensionManager(KeyValueStorage globalKvs, IEnumerable<T> extensions, 
            string pluralName)
        {
            this.PluralName = pluralName;

            var featuresList = new List<ExtensionFeature<T>>();
            this.Features = featuresList;
            var children = new List<ISettingsGroup>();

            foreach (T ext in extensions)
            {
                string prefix = $"{pluralName}:" + ext.GetType().FullName + ":";
                KeyValueStorage kvs = new PrefixedKeyValueStorage(globalKvs, prefix);

                SettingsGroup settingsGroup = new SettingsGroup(ext.Name, ext, kvs);

                featuresList.Add(new ExtensionFeature<T>(ext, settingsGroup));
                
                children.Add(settingsGroup);
            }

            this.SettingsGroup = new SettingsGroup(pluralName, children);
        }
    }
}
