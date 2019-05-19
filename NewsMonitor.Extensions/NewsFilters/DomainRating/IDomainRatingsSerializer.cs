using NewsMonitor.WPF.Settings.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.Extensions.NewsFilters.DomainRating
{
    public interface IDomainRatingsSerializer : IListSerializer
    {
        string Serialize(List<DomainRating> domainRatings);

        List<DomainRating> DeserializeTyped(string str);
    }
}
