using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.WPF.Settings.Mappings
{
    public interface IListSerializer
    {
        string Serialize(IEnumerable domainRatings);

        IEnumerable Deserialize(string str);
    }
}
