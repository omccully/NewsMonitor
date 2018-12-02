using NewsMonitor.WPF.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NewsMonitor.WPF.Views
{
    /// <summary>
    /// Interaction logic for ShareJobStatusBar.xaml
    /// </summary>
    public partial class ShareJobStatusBar : UserControl
    {
        ShareJobQueue ShareJobQueue;

        public event EventHandler<ShareJobFinishedEventArgs> JobFinished;

        public ShareJobStatusBar()
        {
            InitializeComponent();

            ShareJobQueue = new ShareJobQueue();
            ShareJobQueue.JobStarted += ShareJobQueue_CurrentJobStatusUpdate;
            ShareJobQueue.JobFinished += ShareJobQueue_CurrentJobStatusUpdate;
            ShareJobQueue.CurrentJobStatusUpdate += ShareJobQueue_CurrentJobStatusUpdate;
            ShareJobQueue.AllJobsFinished += ShareJobQueue_AllJobsFinished;
            
        }

        public void AddUnfinishedJobs(IEnumerable<IShareJob> unfinishedJobs)
        {
            Console.WriteLine("UnfinishedJobs.Count == " + unfinishedJobs.Count());
            foreach (IShareJob job in unfinishedJobs)
            {
                job.Finished += Job_Finished;
                Console.WriteLine(job);
            }

            ShareJobQueue.Enqueue(unfinishedJobs);
        }

        private void Job_Finished(object sender, ShareJobFinishedEventArgs e)
        {
            // TODO: make this pass the IShareJob as well
            JobFinished?.Invoke(this, e); 
        }

        private void ShareJobQueue_CurrentJobStatusUpdate(object sender, ShareJobQueueStatusEventArgs e)
        {
            // update status bar based on queue status
            string message = $"{ShareJobQueue.Count} jobs in queue -- " +
                $"{e.Job.Description}: {e.Status}";
            Console.WriteLine(message);
            JobStatusTextBlock.Text = message;
            JobStatusTextBlock.Foreground = new SolidColorBrush(e.Failed ? Colors.Red : Colors.Black);
        }

        private void ShareJobQueue_AllJobsFinished(object sender, EventArgs e)
        {
            JobStatusTextBlock.Text = "";
        }



        

        private void SkipJobButton_Click(object sender, RoutedEventArgs e)
        {
            // remove the job from the extension's KVS
            IShareJob currentJob = ShareJobQueue.CurrentJob;
            currentJob?.Cancel();

            // continue on with the queue
            ShareJobQueue.RunAllJobs();
        }
    }
}
