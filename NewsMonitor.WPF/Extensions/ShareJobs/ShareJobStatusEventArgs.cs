using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.WPF.Extensions
{
    public class ShareJobStatusEventArgs : EventArgs
    {
        public string Message { get; private set; }

        public ShareJobStatusEventArgs(string message)
        {
            this.Message = message;
        }
    }
}
