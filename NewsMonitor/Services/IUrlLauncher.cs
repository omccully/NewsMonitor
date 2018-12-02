using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.Services
{
    public interface IUrlLauncher
    {
        void Launch(string url);
    }
}
