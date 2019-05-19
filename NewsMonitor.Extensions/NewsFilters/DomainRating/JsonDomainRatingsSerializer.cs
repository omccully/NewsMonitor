using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.Extensions.NewsFilters.DomainRating
{
    class JsonDomainRatingsSerializer : IDomainRatingsSerializer
    {
        public string Serialize(List<DomainRating> domainRatings)
        {
            return Serialize((IEnumerable)domainRatings);
        }

        public string Serialize(IEnumerable domainRatings)
        {
            return JsonConvert.SerializeObject(domainRatings);
        }

        public IEnumerable Deserialize(string str)
        {
            return DeserializeTyped(str);
        }

        public List<DomainRating> DeserializeTyped(string str)
        {
            return JsonConvert.DeserializeObject<List<DomainRating>>(str);
        }
    }
}
