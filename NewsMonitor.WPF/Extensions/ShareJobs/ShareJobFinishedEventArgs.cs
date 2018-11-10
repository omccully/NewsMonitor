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

        public bool WasCancelled { get; set; }

        public string ErrorMessage { get; set; }

        public static ShareJobFinishedEventArgs Cancel
        {
            get
            {
                return new ShareJobFinishedEventArgs()
                {
                    WasCancelled = true
                };
            }
        }

        public static ShareJobFinishedEventArgs Error(string message)
        {
            return new ShareJobFinishedEventArgs()
            {
                ErrorMessage = message
            };
        }
    }
}
