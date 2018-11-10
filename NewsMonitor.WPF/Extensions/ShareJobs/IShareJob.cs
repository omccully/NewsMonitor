using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.WPF.Extensions
{
    public interface IShareJob
    {
        string Description { get; }
       // event EventHandler Started;
        event EventHandler<ShareJobStatusEventArgs> StatusUpdate;
        event EventHandler<ShareJobFinishedEventArgs> Finished;
        void Skip();

        Task Execute();
    }
}
