using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.Extensions.NewsFilters.DomainRating
{
    public class DomainRating
    {
        public string Domain { get; set; }

        public long MonthlyVisitors { get; set; }

        public DomainRatingDecision Decision { get; set; }

        public DateTime LastChecked { get; set; }

        public override string ToString()
        {
            return $"Monthly visitors = {MonthlyVisitors}";
        }
    }
}
