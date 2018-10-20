using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.WPF.Extensions
{
    public class JobsCreatedEventArgs
    {
        public IEnumerable<ISharerJob> Jobs { get; private set; }

        public JobsCreatedEventArgs(IEnumerable<ISharerJob> jobs)
        {
            this.Jobs = new List<ISharerJob>(jobs);
        }
    }
}
