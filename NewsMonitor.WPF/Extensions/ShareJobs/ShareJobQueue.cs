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

        public event EventHandler<ShareJobQueueStatusEventArgs> JobStarted;
        public event EventHandler<ShareJobQueueStatusEventArgs> JobFinished;
        public event EventHandler<ShareJobQueueStatusEventArgs> 
            CurrentJobStatusUpdate;
        public event EventHandler AllJobsFinished;

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
                return InnerQueue.Count() + 
                    (IsRunningJob ? 1 : 0);
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

        object RunAllJobsLock = new object();
        public async void RunAllJobs()
        {
            if (IsRunningJob) return;

            while (true)
            {
                IShareJob nextJob = null;

                // TryDequeue is thread-safe
                if (!InnerQueue.TryDequeue(out nextJob))
                {
                    AllJobsFinished?.Invoke(this, new EventArgs());
                    return;
                }

                nextJob.StatusUpdate += Job_StatusUpdate;
                nextJob.Finished += NextJob_Finished;

                IsRunningJob = true;
                CurrentJob = nextJob;

                JobStarted?.Invoke(this, new ShareJobQueueStatusEventArgs(nextJob, "Started"));

                await nextJob.Execute(); // executes NextJob_Finished when finished

                IsRunningJob = false;
                CurrentJob.StatusUpdate -= Job_StatusUpdate;
            }
            
        }

        private void NextJob_Finished(object sender, ShareJobFinishedEventArgs e)
        {
            IShareJob job = (IShareJob)sender;
            if (e.ErrorMessage != null)
            {
                JobFinished?.Invoke(this, new ShareJobQueueStatusEventArgs(job, "Failed: " + e.ErrorMessage, true));
            }
            else
            {
                JobFinished?.Invoke(this, new ShareJobQueueStatusEventArgs(job, "Finished"));
            }
        }

        private void Job_StatusUpdate(object sender, ShareJobStatusEventArgs e)
        {
            IShareJob job = (IShareJob)sender;
            ShareJobQueueStatusEventArgs ee = new ShareJobQueueStatusEventArgs(job, e.Message);
            CurrentJobStatusUpdate?.Invoke(this, ee);
        }
    }
}
