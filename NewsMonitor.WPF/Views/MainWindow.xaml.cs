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

namespace NewsMonitor.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DatabaseContext dbContext;
        SettingsManager SettingsManager;

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
                    NewsArticlesPage = new NewsArticlesPage(value,
                        SettingsManager.FilterExtensionManager.Features,
                        SettingsManager.SharerExtensionManager.Features);
                    NewsArticlesPage.ShareJobsCreated += NewsArticlesPage_JobsCreated;
                    NewsArticlesPage.NewsArticleModified += NewsArticlesPage_NewsArticleModified;
                    NewsArticlesPageFrame.Navigate(NewsArticlesPage);

                    NewsArticlesPageFrame.Navigating += NewsArticlesPageFrame_Navigating;
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
            foreach (NewsArticle article in NewsArticlesPage.SelectedNewsArticles)
            {
                bool passedAll = SearchTerms.All(st => PassesAllFilters(article, st));

                if (!passedAll) {
                    
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
