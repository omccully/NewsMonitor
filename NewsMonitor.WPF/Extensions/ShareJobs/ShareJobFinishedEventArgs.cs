using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.WPF.Extensions
{
    public class ShareJobFinishedEventArgs : EventArgs
    {
        public IShareJob Job { get; private set; }
        public string Url { get; private set; }

        public ShareJobFinishedEventArgs(IShareJob job, string url = null)
        {
            this.Job = job;
            this.Url = url;
        }

        public bool WasCancelled { get; set; }

        public string ErrorMessage { get; set; }

        public static ShareJobFinishedEventArgs Cancel(IShareJob job)
        {
            return new ShareJobFinishedEventArgs(job)
            {
                WasCancelled = true
            };
        }

        public static ShareJobFinishedEventArgs Error(IShareJob job, string message)
        {
            return new ShareJobFinishedEventArgs(job)
            {
                ErrorMessage = message
            };
        }
    }
}
