using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.WPF.Extensions
{
    public class ShareJobQueueStatusEventArgs : EventArgs
    {
        public IShareJob Job { get; private set; }
        public string Status { get; private set; }
        public bool Failed { get; private set; }

        public ShareJobQueueStatusEventArgs(IShareJob job, string status, bool failed=false)
        {
            this.Job = job;
            this.Status = status;
            this.Failed = failed;
        }
    }
}
