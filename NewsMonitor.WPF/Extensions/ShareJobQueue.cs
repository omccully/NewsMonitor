using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.WPF.Extensions
{
    public class ShareJobQueue
    {
        ConcurrentQueue<IShareJob> InnerQueue = 
            new ConcurrentQueue<IShareJob>();

        public event EventHandler<ShareJobStatusEventArgs> JobStarted;
        public event EventHandler<ShareJobStatusEventArgs> JobFinished;
        public event EventHandler<ShareJobStatusEventArgs> 
            CurrentJobStatusUpdate;

        public ShareJobQueue()
        {

        }

        #region Thread-safe queue operations
        object queueLock = new object();
        public void Enqueue(IShareJob shareJob)
        {
            InnerQueue.Enqueue(shareJob);

            NotifyJobEnqueued();
        }

        public void Enqueue(IEnumerable<IShareJob> shareJobs)
        {
            foreach (IShareJob job in shareJobs)
            {
                InnerQueue.Enqueue(job);
            }

            NotifyJobEnqueued();
        }

        public int Count
        {
            get
            {
                return InnerQueue.Count();
            }
        }

        #endregion

        public bool IsRunningJob { get; private set; }

        object shareJobLock = new object();
        IShareJob _CurrentJob = null;
        public IShareJob CurrentJob
        {
            get
            {
                lock(shareJobLock)
                {
                    return _CurrentJob;
                }
            }
            private set
            {
                lock (shareJobLock)
                {
                    _CurrentJob = value;
                }
            }
        }

        object notifyQueueChangedLock = new object();
        void NotifyJobEnqueued()
        {
            if (!IsRunningJob)
            {
                RunAllJobs();
            }

            // otherwise, if IsRunningJob, this new job
            // will be executed when the others are done
        }

        async void RunAllJobs()
        {
            while (true)
            {
                Task jobTask = null;

                IShareJob nextJob = null;

                // TryDequeue is thread-safe
                if (!InnerQueue.TryDequeue(out nextJob))
                {
                    IsRunningJob = false;
                    CurrentJob = null;
                    return;
                }

                IsRunningJob = true;

                nextJob.StatusUpdate += Job_StatusUpdate;
                CurrentJob = nextJob;
                JobStarted?.Invoke(this, new ShareJobStatusEventArgs(nextJob, "Started"));
                jobTask = nextJob.Execute();
                    
                await jobTask;
                CurrentJob.StatusUpdate -= Job_StatusUpdate;
                JobFinished?.Invoke(this, new ShareJobStatusEventArgs(CurrentJob, "Finished"));
            }
        }

        private void Job_StatusUpdate(object sender, ShareJobStatusEventArgs e)
        {
            CurrentJobStatusUpdate?.Invoke(this, e);
        }
    }
}
