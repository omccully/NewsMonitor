using NewsMonitor.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.WPF.Extensions
{
    public class NeedsAttentionEventArgs : EventArgs
    {
        public ShareJobResult Result { get; }
        public string Message { get; }

        public NeedsAttentionEventArgs(ShareJobResult result, string message)
        {
            Result = result;
            this.Message = message;
        }
    }
}
