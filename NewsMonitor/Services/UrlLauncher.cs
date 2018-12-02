using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.Services
{
    public class UrlLauncher : IUrlLauncher
    {
        public void Launch(string url)
        {
            if (url != null && (url.StartsWith("http://") || url.StartsWith("https://")))
            {
                Process.Start(url);
            }
        }
    }
}
