using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.WPF.Extensions
{
    public interface IDomainRater
    {
        long GetMonthlyVisitors(string domain);
    }
}
