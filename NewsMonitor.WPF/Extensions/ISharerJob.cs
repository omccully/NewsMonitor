using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.WPF.Extensions
{
    public interface ISharerJob
    {
        string Description { get; }
        event EventHandler<SharerJobStatusEventArgs> StatusUpdate;
        event EventHandler Finished;

        Task Execute();
    }
}
