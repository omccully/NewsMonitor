using NewsMonitor.Data.Database;
using NewsMonitor.Data.Models;
using NewsMonitor.Services.NewsSearchers;
using NewsMonitor.WPF.Extensions;
using System;
using System.Windows;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NewsMonitor.Helpers;
using NewsMonitor.WPF.Settings;
using NewsMonitor.WPF.Views;
using System.Configuration;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows.Documents;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Timers;
using NewsMonitor.Services;
using NewsMonitor.WPF.MachineLearning;

namespace NewsMonitor.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DatabaseContext dbContext;
        SettingsManager SettingsManager;

        NewsArticleRatingPredictor _newsArticleRatingPredictor;

        ObservableCollection<NewsArticle> _AllNewsArticles;   
        ObservableCollection<NewsArticle> AllNewsArticles
        {
            get
            {
                return _AllNewsArticles;
            }
            set
            {
                if(value != null)
                {
                    NewsArticlesPage = new NewsArticlesPage(dbContext, value,
                        SettingsManager.FilterExtensionManager.Features,
                        SettingsManager.SharerExtensionManager.Features);
                    NewsArticlesPage.ShareJobsCreated += NewsArticlesPage_JobsCreated;
                    NewsArticlesPage.NewsArticleModified += NewsArticlesPage_NewsArticleModified;
                    NewsArticlesPage.QuickFilterFinished += NewsArticlesPage_QuickFilterFinished;
                    NewsArticlesPageFrame.Navigate(NewsArticlesPage);

                    NewsArticlesPageFrame.Navigating += NewsArticlesPageFrame_Navigating;

                    _newsArticleRatingPredictor = new NewsArticleRatingPredictor(value);
                }
                
                _AllNewsArticles = value;
            }
        }

        ObservableCollection<ShareJobResult> _AllShareJobResults;
        ObservableCollection<ShareJobResult> AllShareJobResults
        {
            get
            {
                return _AllShareJobResults;
            }
            set
            {
                if(value != null)
                {
                    ShareHistoryPageFrame.Navigate(new ShareHistoryPage(value));
                    ShareHistoryPageFrame.Navigating += NewsArticlesPageFrame_Navigating;
                    value.CollectionChanged += AllShareJobResults_CollectionChanged;
                }
                   
                _AllShareJobResults = value;
            }
        }

        private void AllShareJobResults_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null) return;
            foreach(var newItem in e.NewItems)
            {
                dbContext.ShareJobResults.Add((ShareJobResult)newItem);
            }
            
            dbContext.SaveChangesAsync();
        }

        NewsArticlesPage NewsArticlesPage;

        #region Initialization
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                InitializeFromDb();

                InitializeStatusBar();

                InitializeProgressBar();

                //InitializeMonitor();

                this.SizeToContent = SizeToContent.Width;
            }
            catch(DataException e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MessageBox.Show(
                    String.Join(Environment.NewLine, ExceptionMessages(e)));
                this.Close();
            }
            catch (InvalidOperationException e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MessageBox.Show(e.Message);
                this.Close();
            }
        }

        TimeSpan _monitorInterval = TimeSpan.FromMinutes(15);
        Timer timer;
        void InitializeMonitor()
        {
            Timer_Elapsed(null, null);
            timer = new Timer(_monitorInterval.TotalMilliseconds);
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        bool ShareJobAppliesToPostMonitor(ShareJobResult shareJobResult, IPostMonitorExtension postMonitor)
        {
            TimeSpan timeSinceShareJob = DateTime.Now - shareJobResult.Time;

            if (shareJobResult.Url == null) return false;
            Uri uri = new Uri(shareJobResult.Url);
            return uri.Host.EndsWith(postMonitor.Domain) &&
                timeSinceShareJob < postMonitor.TimeSpan;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Monitoring...");
            foreach(ExtensionFeature<IPostMonitorExtension> postMonitor in PostMonitors)
            {
                IEnumerable<ShareJobResult> applicableResults = dbContext
                    .ShareJobResults.ToList().Where(sjr =>
                        ShareJobAppliesToPostMonitor(sjr, postMonitor.Extension))
                    .OrderByDescending(sjr => sjr.Time).ToList();

                KeyValueStorage kvs = postMonitor.KeyValueStorage;
                if(kvs != null)
                {
                    string shareSettingsWith = postMonitor.Extension.ShareSettingsWith;
                    if (shareSettingsWith != null)
                    {
                        var sharer = SettingsManager.SharerExtensionManager.Features
                            .FirstOrDefault(f => f.Extension.Name == shareSettingsWith);
                        kvs = sharer.KeyValueStorage;
                    }
                }

                postMonitor.Extension.Monitor(applicableResults, kvs);
            }
        }

        void InitializeProgressBar()
        {
            FindArticlesProgressBar.NewsSearchers = NewsSearchers;
            FindArticlesProgressBar.FilterFeatures = SettingsManager.FilterExtensionManager.Features;
            FindArticlesProgressBar.ProvideSearchTerms = () => SearchTerms;
            FindArticlesProgressBar.ArticlesFound += FindArticlesProgressBar_ArticlesFound;
        }

        void InitializeStatusBar()
        {
            IEnumerable<IShareJob> unfinishedJobs = SettingsManager
                   .SharerExtensionManager.Features
                   .SelectMany(f => f.Extension.GetUnfinishedJobs(f.KeyValueStorage));
            ShareJobStatusBar.AddUnfinishedJobs(unfinishedJobs);
            ShareJobStatusBar.JobFinished += ShareJobStatusBar_JobFinished;
        }

        private void ShareJobStatusBar_JobFinished(object sender, ShareJobFinishedEventArgs e)
        {
            AllShareJobResults.Add(
                new ShareJobResult(e.Job.Description,
                e.Url, e.WasCancelled, e.ErrorMessage));
        }

        private void FindArticlesProgressBar_ArticlesFound(object sender, Services.NewsArticlesFoundEventArgs e)
        {
            foreach (NewsArticle article in e.NewsArticles)
            {
                if (ExistingArticleUrls.Contains(article.Url)) continue;

                AllNewsArticles.Add(article);

                dbContext.NewsArticles.Add(article);
                dbContext.SaveChangesAsync();

                Console.WriteLine(article);
            }
        }

        private void NewsArticlesPage_NewsArticleModified(object sender, EventArgs e)
        {
            dbContext.SaveChangesAsync();
        }

        List<string> ExceptionMessages(Exception e, List<string> messageList = null)
        {
            if (messageList == null) messageList = new List<string>();

            messageList.Add(e.Message);

            if(e.InnerException != null)
            {
                ExceptionMessages(e.InnerException, messageList);
            }

            return messageList;
        }


        void InitializeFromDb()
        {
            dbContext = new DatabaseContext("NewsMonitorDb");

            SettingsManager = new SettingsManager(
                    new DatabaseKeyValueStorage(dbContext));

            AllNewsArticles = new ObservableCollection<NewsArticle>(
                dbContext.NewsArticles.ToList());
            ExistingArticleUrls = new ObservableCollectionSearcher<NewsArticle, string>(
                AllNewsArticles, (article) => article.Url);
            
            AllShareJobResults = new ObservableCollection<ShareJobResult>(dbContext.ShareJobResults.ToList());
        }
        #endregion

        ObservableCollectionSearcher<NewsArticle, string> ExistingArticleUrls;

        IEnumerable<ExtensionFeature<IPostMonitorExtension>> _postMonitors;
        IEnumerable<ExtensionFeature<IPostMonitorExtension>> PostMonitors
        {
            get
            {
                if(_postMonitors == null)
                {
                    _postMonitors = SettingsManager.PostMonitorExtensionManager
                        .Features.ToList();
                    foreach(ExtensionFeature<IPostMonitorExtension> feature in _postMonitors)
                    {
                        feature.Extension.NeedsAttention += PostMonitor_NeedsAttention;
                    }
                }
                return _postMonitors;
            }
        }

        private void PostMonitor_NeedsAttention(object sender, NeedsAttentionEventArgs e)
        {
           Dispatcher.BeginInvoke((Action)delegate
           {
               string messageText = e.Result.Url + "\n\n" + e.Message + "\n\n" +
                   "Would you like to launch this URL now?";
               MessageBoxResult dialogResult = MessageBox.Show(messageText, "Needs attention", 
                   MessageBoxButton.YesNo);
               if (dialogResult == MessageBoxResult.Yes)
               {
                   new UrlLauncher().Launch(e.Result.Url);
               }
           });
        }

        IEnumerable<INewsSearcher> NewsSearchers
        {
            get
            {
                return SettingsManager.SearcherExtensionManager.Features
                    .Select(f => f.Extension.CreateNewsSearcher(f.KeyValueStorage));
            }
        }

        bool PassesAllFilters(NewsArticle newsArticle, string searchTerm)
        {
            return SettingsManager.FilterExtensionManager.Features.All(
                f => f.Extension.AllowArticle(newsArticle, searchTerm, f.KeyValueStorage));
        }

        IEnumerable<string> SearchTerms
        {
            get
            {
                string searchTermsStr = SettingsManager.General
                    .KeyValueStorage.GetString(GeneralSettingsPage.SearchTermsKey);
                return searchTermsStr.Split(',').Select(str => str.Trim());
            }
        }

        private async void FindArticlesButton_Click(object sender, RoutedEventArgs e)
        {
            FindArticlesButton.IsEnabled = false;

            await FindArticlesProgressBar.FindArticlesAsync();

            FindArticlesButton.IsEnabled = true;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow sw = new SettingsWindow(SettingsManager.SettingsGroups);
            sw.ShowDialog();
            Console.WriteLine("returned to main window");
        }

        private void RefilterButton_Click(object sender, RoutedEventArgs e)
        {
            Refilter(NewsArticlesPage.SelectedNewsArticles);
        }

        private void NewsArticlesPage_QuickFilterFinished(object sender, EventArgs e)
        {
            //Refilter(NewsArticlesPage.SelectedNewsArticles);
        }

        private void RefilterAllButton_Click(object sender, RoutedEventArgs e)
        {
            Refilter(AllNewsArticles);
        }

        private void RepredictRatingButton_Click(object sender, RoutedEventArgs e)
        {
            foreach(NewsArticle article in NewsArticlesPage.SelectedNewsArticles)
            {
                if (article.UserSetRating) continue;

                int newRating = _newsArticleRatingPredictor.Predict(article);
                bool ratingChanged = newRating != article.Rating;
                article.Rating = newRating;
                if(ratingChanged)
                {
                    dbContext.SaveChanges();
                }
            }
        }

        private void Refilter(IEnumerable<NewsArticle> articles)
        {
            // AllNewsArticles.Where(a => !a.Hidden)
            foreach (NewsArticle article in articles.Where(a => !a.Hidden))
            {
                bool passedAll = SearchTerms.All(st => PassesAllFilters(article, st));

                if (!passedAll)
                {
                    article.Hidden = true;

                    dbContext.SaveChanges();
                }
            }
        }

        private void NewsArticlesPage_JobsCreated(object sender, JobsCreatedEventArgs e)
        {
            ShareJobStatusBar.AddUnfinishedJobs(e.Jobs);
        }

        private void NewsArticlesPageFrame_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
