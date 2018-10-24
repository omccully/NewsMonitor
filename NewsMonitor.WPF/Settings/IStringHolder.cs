using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.WPF.Settings
{
    public interface IStringSerializable
    {
        void Deserialize(string val);

        string Serialize();
    }
}
