using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.Extensions.NewsFilters.RegexTitle
{
    interface IStringSectionsSerializer
    {
        string Serialize(Dictionary<string, IEnumerable<string>> dict);

        Dictionary<string, IEnumerable<string>> Deserialize(string str);
    }
}
