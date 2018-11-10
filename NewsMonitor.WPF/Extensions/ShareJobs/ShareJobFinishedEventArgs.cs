using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.WPF.Extensions
{
    public class ShareJobFinishedEventArgs : EventArgs
    {
        public string Url { get; private set; }

        public ShareJobFinishedEventArgs(string url = null)
        {
            this.Url = url;
        }

        public bool WasSkipped { get; set; }

        public static ShareJobFinishedEventArgs Skipped
        {
            get
            {
                return new ShareJobFinishedEventArgs()
                {
                    WasSkipped = true
                };
            }
        }
    }
}
