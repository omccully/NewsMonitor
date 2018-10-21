using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.WPF.Extensions
{
    public class ShareJobStatusEventArgs : EventArgs
    {
        public IShareJob Job { get; private set; }
        public string Status { get; private set; }

        public ShareJobStatusEventArgs(IShareJob job, string status)
        {
            this.Job = job;
            this.Status = status;
        }

    }
}
