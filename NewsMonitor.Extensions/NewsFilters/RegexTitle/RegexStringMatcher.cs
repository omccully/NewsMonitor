using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NewsMonitor.Extensions.NewsFilters.RegexTitle
{
    class RegexStringMatcher : IStringMatcher
    {
        public bool Matches(string str, string matcher)
        {
            if (str.ToLower() == matcher.ToLower()) return true;
            try
            {
                Regex regex = new Regex(matcher, RegexOptions.IgnoreCase);
                return regex.IsMatch(str);
            }
            catch
            {
                return false;
            }
        }
    }
}
