using NewsMonitor.WPF.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NewsMonitor.WPF.Views
{
    public abstract class NewsSharerWindow : Window
    {
        public event EventHandler<JobsCreatedEventArgs> JobsCreated;
    }
}
