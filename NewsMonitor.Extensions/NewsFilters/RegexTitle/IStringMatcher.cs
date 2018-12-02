using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.Extensions.NewsFilters.RegexTitle
{
    public interface IStringMatcher
    {
        bool Matches(string str, string matcher);
    }
}
