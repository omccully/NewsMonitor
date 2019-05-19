using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.Extensions.NewsFilters.DomainRating
{
    public enum DomainRatingDecision
    {
        Default,
        Blacklisted,
        Whitelisted
    }
}
