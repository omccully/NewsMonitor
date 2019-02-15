using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NewsMonitor.WPF.Extensions
{
    public abstract class ShareJob : IShareJob
    {
        public abstract string Description { get; }

        public event EventHandler<ShareJobStatusEventArgs> StatusUpdate;
        public event EventHandler<ShareJobFinishedEventArgs> Finished;

        protected void OnStatusUpdate(ShareJobStatusEventArgs ea)
        {
            StatusUpdate.Invoke(this, ea);
        }

        [XmlIgnore]
        bool finished = false;

        private void OnFinished(ShareJobFinishedEventArgs ea)
        {
            if(!finished)
            {
                Finished?.Invoke(this, ea);
                finished = true;
            }
        }

        public async Task Execute()
        {
            try
            {
                string url = await InnerExecute();
                OnFinished(new ShareJobFinishedEventArgs(this, url));
            }
            catch (Exception e)
            {
                OnFinished(ShareJobFinishedEventArgs.Error(this, e.Message));
            }
        }
        
        protected abstract Task<string> InnerExecute();

        public void Cancel()
        { 
            OnFinished(ShareJobFinishedEventArgs.Cancel(this));
        }

        public override string ToString()
        {
            return Description;
        }
    }
}
