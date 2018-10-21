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
using System.Windows.Shapes;
using NewsMonitor.Data.Database;
using NewsMonitor.Data.Models;
using NewsMonitor.WPF.Views;
using NewsMonitor.WPF.Extensions;

namespace NewsMonitor.Extensions.NewsSharers.Reddit
{
    /// <summary>
    /// Interaction logic for RedditNewsSharerWindow.xaml
    /// </summary>
    public partial class RedditNewsSharerWindow : NewsSharerWindow
    {
        //NewsArticle newsArticle

        public RedditNewsSharerWindow(NewsArticle newsArticle, KeyValueStorage kvs /*, RedditSharp.Reddit reddit*/)
        {
            InitializeComponent();
        }

        private void PostButton_Click(object sender, RoutedEventArgs e)
        {
            base.OnJobsCreated(new JobsCreatedEventArgs(new List<IShareJob>()
            {
                //new 
            }));
        }
    }
}
