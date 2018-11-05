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

namespace NewsMonitor.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DatabaseContext dbContext;
        SettingsManager SettingsManager;

        ObservableCollection<NewsArticle> AllNewsArticles;
        ObservableCollectionFilter<NewsArticle> NewsArticleFilter;

        ShareJobQueue ShareJobQueue;

        ReadOnlyObservableCollection<NewsArticle> NewsArticlesSource;


        public MainWindow()
        {
            InitializeComponent();

            try
            {
                InitializeFromDb();

                this.SettingsManager = new SettingsManager(
                    new DatabaseKeyValueStorage(dbContext));

                IEnumerable<IShareJob> unfinishedJobs = SettingsManager
                    .SharerExtensionManager.Features
                    .SelectMany(f => f.Extension.GetUnfinishedJobs(f.KeyValueStorage));

                Console.WriteLine("UnfinishedJobs.Count == " + unfinishedJobs.Count());
                foreach(IShareJob job in unfinishedJobs)
                {
                    Console.WriteLine(job);
                }

                ShareJobQueue = new ShareJobQueue();
                ShareJobQueue.JobStarted += ShareJobQueue_CurrentJobStatusUpdate;
                ShareJobQueue.JobFinished += ShareJobQueue_CurrentJobStatusUpdate;
                ShareJobQueue.CurrentJobStatusUpdate += ShareJobQueue_CurrentJobStatusUpdate;
                ShareJobQueue.AllJobsFinished += ShareJobQueue_AllJobsFinished;
                ShareJobQueue.Enqueue(unfinishedJobs);
            }
            catch(DataException e)
            {
                MessageBox.Show(
                    String.Join(Environment.NewLine, ExceptionMessages(e)));
                this.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                this.Close();
            }
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
            AllNewsArticles = new ObservableCollection<NewsArticle>(dbContext.NewsArticles.ToList());
            NewsArticleFilter = new ObservableCollectionFilter<NewsArticle>(AllNewsArticles, 
                (na) => !na.Hidden);

            NewsArticlesSource = NewsArticleFilter.FilteredResults;

            NewsArticlesDataGrid.ItemsSource = NewsArticleFilter.FilteredResults;
            this.SizeToContent = SizeToContent.Width;
        }

        ReadOnlyObservableCollection<NewsArticle> GetNewsArticles()
        {
            return NewsArticleFilter.FilteredResults;
        }

        HashSet<string> ExistingArticleUrls
        {
            get
            {
                HashSet<string> existing_article_urls = new HashSet<string>();
                foreach (NewsArticle na in AllNewsArticles)
                {
                    existing_article_urls.Add(na.Url);
                }
                return existing_article_urls;
            }
        }

        IEnumerable<INewsSearcher> NewsSearchers
        {
            get
            {
                List<INewsSearcher> results = new List<INewsSearcher>();

                foreach (ExtensionFeature<INewsSearcherExtension> extFeature in
                    SettingsManager.SearcherExtensionManager.Features)
                {
                    INewsSearcher searcher = extFeature.Extension.CreateNewsSearcher(
                        extFeature.SettingsGroup.KeyValueStorage);

                    results.Add(searcher);
                }
                return results;
            }
        }

        bool PassesAllFilters(NewsArticle newsArticle, string searchTerm)
        {
            return SettingsManager.FilterExtensionManager.Features.All(
                f => f.Extension.AllowArticle(newsArticle, searchTerm, f.SettingsGroup.KeyValueStorage));
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

        IEnumerable<NewsArticle> SelectedNewsArticles
        {
            get
            {
                return NewsArticlesDataGrid.SelectedItems.Cast<NewsArticle>().ToList();
            }
        }


        private void FindArticlesButton_Click(object sender, RoutedEventArgs e)
        {
            FindArticlesButton.IsEnabled = false;

            try
            {
                IEnumerable<INewsSearcher> news_searchers = NewsSearchers;
                HashSet<string> existing_article_urls = ExistingArticleUrls;

                IEnumerable<string> search_terms = SearchTerms;

                foreach (string search_term in search_terms)
                {
                    Console.WriteLine("Search Term=" + search_term);
                    foreach(INewsSearcher news_searcher in news_searchers)
                    {
                        Console.WriteLine("News Searcher=" + news_searcher.GetType());
                   
                        IEnumerable<NewsArticle> results = news_searcher.Search(search_term)
                            .Where(article => PassesAllFilters(article, search_term));
                        foreach (NewsArticle article in results)
                        {
                            if (existing_article_urls.Contains(article.Url)) continue;

                            AllNewsArticles.Add(article);
                            existing_article_urls.Add(article.Url);
                            dbContext.NewsArticles.Add(article);
                            dbContext.SaveChanges();

                            Console.WriteLine(article);
                        }
                    }
                }
                Console.WriteLine("finished");
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                
                return;
            }
            finally
            {
                FindArticlesButton.IsEnabled = true;
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow sw = new SettingsWindow(SettingsManager.SettingsGroups);
            sw.ShowDialog();
            Console.WriteLine("returned to main window");
        }

        #region Delete and refilter
        private void DeleteSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelectedNewsArticles();
        }

        private void NewsArticlesDataGrid_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Delete) return;

            DeleteSelectedNewsArticles();
        }

        void DeleteSelectedNewsArticles()
        {
            foreach (NewsArticle article in SelectedNewsArticles)
            {
                article.Hidden = true;

                dbContext.SaveChanges();
            }
        }

        private void RefilterButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (NewsArticle article in SelectedNewsArticles)
            {
                bool passedAll = SearchTerms.All(st => PassesAllFilters(article, st));

                if (!passedAll) {
                    
                    article.Hidden = true;

                    dbContext.SaveChanges();
                }
            }
        }
        #endregion

        #region Launch article
        void LaunchArticle(NewsArticle article)
        {
            if (article != null &&
                (article.Url.StartsWith("http://") || article.Url.StartsWith("https://")))
            {
                Process.Start(article.Url);
            }
        }

        private void ShowSelectedInBrowser_Click(object sender, RoutedEventArgs e)
        {
            foreach (NewsArticle article in SelectedNewsArticles)
            {
                LaunchArticle(article);
            }
        }

        void OnHyperlinkClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Source = " + e.Source);
            System.Diagnostics.Debug.WriteLine("OriginalSource = " + e.OriginalSource);

            NewsArticle article = ((Hyperlink)e.OriginalSource).DataContext as NewsArticle;

            LaunchArticle(article);
        }
        #endregion

        #region Share
        private void ShareSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Share selected");
            ExtensionFeature<INewsSharerExtension> feature = SettingsManager.SharerExtensionManager.Features.First();
            foreach(NewsArticle article in SelectedNewsArticles)
            {
                NewsSharerWindow sharerWindow =
                    feature.Extension.CreateSharerWindow(article, feature.SettingsGroup.KeyValueStorage);
                sharerWindow.JobsCreated += SharerWindow_JobsCreated;
                sharerWindow.ShowDialog();
            }
        }

        private void SharerWindow_JobsCreated(object sender, JobsCreatedEventArgs e)
        {
            ShareJobQueue.Enqueue(e.Jobs);
        }

        private void ShareJobQueue_CurrentJobStatusUpdate(object sender, ShareJobStatusEventArgs e)
        {
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

        #endregion
    }
}
