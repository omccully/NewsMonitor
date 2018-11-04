using NewsMonitor.Data.Database;
using NewsMonitor.Data.Models;
using NewsMonitor.WPF.Extensions;
using NewsMonitor.WPF.Settings.Mappings;
using NewsMonitor.WPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsMonitor.WPF.Views.EditableTreeView;
using System.Text.RegularExpressions;

namespace NewsMonitor.Extensions.NewsFilters.RegexTitle
{
    public class RegexTitleNewsFilterExtension : INewsFilterExtension
    {
        public string Name => "Regex Title Filter";

        TreeModel<string> Model;
        TreeModelSettingsMapping Mapping;

        public const string AnySection = "any";

        public RegexTitleNewsFilterExtension()
        {
            Model = new TreeModel<string>();
            Mapping = new TreeModelSettingsMapping(RegexTitleNewsFilterSettingsPage.RegexTitleTreeKey, null, Model);
        }

        public bool AllowArticle(NewsArticle newsArticle, string searchTerm, KeyValueStorage storage)
        {
            Mapping.Load(storage);
            Dictionary<string, IEnumerable<string>> titleMatcherLookup = Model.GetNextLayerDictionary();

            // TODO: use HashSet as the IEnumerable for better performance
            // then only do the regex in the loop

            return !StringMatchesSection(newsArticle.Title, titleMatcherLookup, searchTerm) &&
                !StringMatchesSection(newsArticle.Title, titleMatcherLookup, AnySection);
        }

        bool StringMatchesSection(string str, Dictionary<string, IEnumerable<string>> titleMatcherLookup, string section)
        {
            if (!titleMatcherLookup.ContainsKey(AnySection)) return true;

            return StringMatchesAny(str, titleMatcherLookup[AnySection]);
        }

        bool StringMatchesAny(string str, IEnumerable<string> matchers)
        {
            return matchers.Any(m => StringMatchesMatcher(str, m));
        }

        bool StringMatchesMatcher(string str, string matcher)
        {
            if (str == matcher) return true;
            try
            {
                Regex regex = new Regex(matcher);
                return regex.IsMatch(str);
            }
            catch
            {
                return false;
            }
        }



        public SettingsPage CreateSettingsPage()
        {
            return new RegexTitleNewsFilterSettingsPage();
        }
    }
}
