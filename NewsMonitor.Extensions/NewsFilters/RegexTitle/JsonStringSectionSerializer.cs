using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NewsMonitor.Extensions.NewsFilters.RegexTitle
{
    class JsonStringSectionSerializer : IStringSectionsSerializer
    {
        public Dictionary<string, IEnumerable<string>> Deserialize(string str)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, IEnumerable<string>>>(str);
        }

        public string Serialize(Dictionary<string, IEnumerable<string>> dict)
        {
            return JsonConvert.SerializeObject(dict);
        }
    }
}
