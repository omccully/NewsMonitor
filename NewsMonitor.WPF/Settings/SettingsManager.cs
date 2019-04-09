using NewsMonitor.Data.Database;
using NewsMonitor.WPF.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
//using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.WPF.Settings
{
    public class SettingsManager
    {
        public IEnumerable<ISettingsGroup> SettingsGroups
        {
            get
            {
                return new List<ISettingsGroup>()
                {
                    General, NewsSearchers, NewsFilters, NewsSharers, PostMonitors
                };
            }
        }

        public ExtensionManager<INewsSearcherExtension> SearcherExtensionManager
            { get; private set; }

        public ExtensionManager<INewsFilterExtension> FilterExtensionManager
            { get; private set; }

        public ExtensionManager<INewsSharerExtension> SharerExtensionManager
            { get; private set; }

        public ExtensionManager<IPostMonitorExtension> PostMonitorExtensionManager
            { get; private set; }

        public SettingsManager(KeyValueStorage globalKvs)
        {
            this.General = new SettingsGroup("General",
                new GeneralSettingsPageFactory(),
                new PrefixedKeyValueStorage(globalKvs, "General:"));

            SearcherExtensionManager =
                new ExtensionManager<INewsSearcherExtension>(globalKvs,
                GetNewsSearcherExtensionsFromConfig(), "News Searchers");
            NewsSearchers = SearcherExtensionManager.SettingsGroup;

            FilterExtensionManager =
               new ExtensionManager<INewsFilterExtension>(globalKvs,
               GetNewsFilterExtensionsFromConfig(), "News Filters");
            NewsFilters = FilterExtensionManager.SettingsGroup;

            SharerExtensionManager =
                new ExtensionManager<INewsSharerExtension>(globalKvs,
                GetNewsSharerExtensionsFromConfig(), "News Sharers");
            NewsSharers = SharerExtensionManager.SettingsGroup;

            PostMonitorExtensionManager =
                new ExtensionManager<IPostMonitorExtension>(globalKvs,
                GetPostMonitorExtensionsFromConfig(), "Post Monitors");
            PostMonitors = PostMonitorExtensionManager.SettingsGroup;
        }

        IEnumerable<INewsSearcherExtension> GetNewsSearcherExtensionsFromConfig()
        {
            return GetExtensionsFromNameValueConfig<INewsSearcherExtension>(
                "newsSearcherExtensions");
        }

        IEnumerable<INewsFilterExtension> GetNewsFilterExtensionsFromConfig()
        {
            return GetExtensionsFromNameValueConfig<INewsFilterExtension>(
                "newsFilterExtensions");
        }
        
        IEnumerable<INewsSharerExtension> GetNewsSharerExtensionsFromConfig()
        {
            return GetExtensionsFromNameValueConfig<INewsSharerExtension>(
                "newsSharerExtensions");
        }

        IEnumerable<IPostMonitorExtension> GetPostMonitorExtensionsFromConfig()
        {
            return GetExtensionsFromNameValueConfig<IPostMonitorExtension>(
                "postMonitorExtensions");
        }

        IEnumerable<T> GetExtensionsFromNameValueConfig<T>(string sectionName)
        {
            var collection = (NameValueCollection)ConfigurationManager
                .GetSection(sectionName);

            if(collection == null)
            {
                throw new InvalidConfigurationException(
                    $"The app.config file must contain the section \"{sectionName}\"");
            }

            return collection.AllKeys.Select(k =>
            {
                string val = collection[k];
                Console.WriteLine($"{k} => {val}");
                Type type = Type.GetType(val);
                if(type == null)
                {
                    throw new InvalidExtensionTypeException($"Type \"{val}\" not found");
                }
                object obj = Activator.CreateInstance(type);

                try
                {
                    return (T)obj;
                }
                catch
                {
                    throw new InvalidExtensionTypeException($"Type \"{val}\" does not inherit from {typeof(T).FullName}");
                }
            });
        }

        public ISettingsGroup General
            { get; private set; }
        public ISettingsGroup NewsSearchers
            { get; private set; }
        public ISettingsGroup NewsFilters
        { get; private set; }
        public ISettingsGroup NewsSharers
            { get; private set; }
        public ISettingsGroup PostMonitors
        { get; private set; }
    }
}
