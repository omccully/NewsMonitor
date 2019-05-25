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

        List<ExtensionFeature<T>> _featuresList;
        KeyValueStorage _globalKvs;
        List<ISettingsGroup> _settingsGroupChildren;

        public ExtensionManager(KeyValueStorage globalKvs, IEnumerable<T> extensions, 
            string pluralName)
        {
            this.PluralName = pluralName;
            _globalKvs = globalKvs;

            _featuresList = new List<ExtensionFeature<T>>();
            this.Features = _featuresList;
            _settingsGroupChildren = new List<ISettingsGroup>();

            foreach (T ext in extensions)
            {
                AddExtension(ext);
            }

            this.SettingsGroup = new SettingsGroup(pluralName, _settingsGroupChildren);
        }

        public void AddExtension(T ext)
        {
            string prefix = $"{PluralName}:" + ext.GetType().FullName + ":";
            KeyValueStorage kvs = new PrefixedKeyValueStorage(_globalKvs, prefix);

            SettingsGroup settingsGroup = new SettingsGroup(ext.Name, ext, kvs);

            _featuresList.Add(new ExtensionFeature<T>(ext, settingsGroup));

            _settingsGroupChildren.Add(settingsGroup);
        }
    }
}
