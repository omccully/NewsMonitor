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

                InitializeDataGridRowContextMenu();

                InitializeJobQueue(unfinishedJobs);
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

        void InitializeDataGridRowContextMenu()
        {
            ContextMenu cm = (ContextMenu)NewsArticlesDataGrid.FindResource("RowMenu");

            MenuItem fmi = cm.Items.Cast<MenuItem>().Where(mi => mi.Name == "FilterMenuItem").First();
            foreach (ExtensionFeature<INewsFilterExtension> filter in SettingsManager.FilterExtensionManager.Features)
            {
                MenuItem mi = new MenuItem() { Header = filter.Extension.Name };
                mi.Click += (o, e) =>
                {
                    foreach(NewsArticle article in SelectedNewsArticles)
                    {
                        System.Diagnostics.Debug.WriteLine(article.Title);
                        FilterNewsArticle(article, filter);
                    }

                    e.Handled = true;
                };
                fmi.Items.Add(mi);
            }


            MenuItem smi = cm.Items.Cast<MenuItem>().Where(mi => mi.Name == "ShareMenuItem").First();
            foreach (ExtensionFeature<INewsSharerExtension> sharer in SettingsManager.SharerExtensionManager.Features)
            {
                MenuItem mi = new MenuItem() { Header = sharer.Extension.Name };
                mi.Click += (o, e) =>
                {
                    foreach (NewsArticle article in SelectedNewsArticles)
                    {
                        System.Diagnostics.Debug.WriteLine(article.Title);
                        ShareNewsArticle(article, sharer);
                    }

                    e.Handled = true;
                };
                smi.Items.Add(mi);
            }
        }

        void InitializeJobQueue(IEnumerable<IShareJob> unfinishedJobs)
        {
            Console.WriteLine("UnfinishedJobs.Count == " + unfinishedJobs.Count());
            foreach (IShareJob job in unfinishedJobs)
            {
                job.Finished += Job_Finished;
                Console.WriteLine(job);
            }

            ShareJobQueue = new ShareJobQueue();
            ShareJobQueue.JobStarted += ShareJobQueue_CurrentJobStatusUpdate;
            ShareJobQueue.JobFinished += ShareJobQueue_CurrentJobStatusUpdate;
            ShareJobQueue.CurrentJobStatusUpdate += ShareJobQueue_CurrentJobStatusUpdate;
            ShareJobQueue.AllJobsFinished += ShareJobQueue_AllJobsFinished;
            ShareJobQueue.Enqueue(unfinishedJobs);
        }

        private void Job_Finished(object sender, ShareJobFinishedEventArgs e)
        {
            IShareJob job = (IShareJob)sender;
            Console.WriteLine(job.Description);
            Console.WriteLine("e.Url=" + e.Url);
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
            FindArticlesAsync();
        }

        async void FindArticlesAsync()
        {
            FindArticlesButton.IsEnabled = false;

            try
            {
                FindArticlesProgressBar.Value = 0;

                IEnumerable<INewsSearcher> news_searchers = NewsSearchers;
                HashSet<string> existing_article_urls = ExistingArticleUrls;

                IEnumerable<string> search_terms = SearchTerms;

                FindArticlesProgressBar.Maximum = news_searchers.Count() * search_terms.Count();

                foreach (string search_term in search_terms)
                {
                    Console.WriteLine("Search Term=" + search_term);
                    foreach (INewsSearcher news_searcher in news_searchers)
                    {
                        Console.WriteLine("News Searcher=" + news_searcher.GetType());

                        IEnumerable<NewsArticle> results = (await news_searcher.SearchAsync(search_term))
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

                        FindArticlesProgressBar.Value++;
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
                FindArticlesProgressBar.Value = 0;
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

        void FilterNewsArticle(NewsArticle newsArticle, ExtensionFeature<INewsFilterExtension> feature = null)
        {
            if (feature == null) feature = SettingsManager.FilterExtensionManager.Features.First();

            Window window = feature.Extension.CreateQuickFilterWindow(newsArticle, feature.SettingsGroup.KeyValueStorage);
            System.Diagnostics.Debug.WriteLine("window = " + window);
            if(window != null) window.ShowDialog();
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
           
            foreach(NewsArticle article in SelectedNewsArticles)
            {
                ShareNewsArticle(article);
            }
        }

        private void DataGridRow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.OriginalSource.GetType());
            System.Diagnostics.Debug.WriteLine(e.Source.GetType());

            NewsArticle article = ((DataGridRow)e.Source).Item as NewsArticle;
            ShareNewsArticle(article);
        }

        void ShareNewsArticle(NewsArticle article, ExtensionFeature<INewsSharerExtension> feature=null)
        {
            if(feature == null) feature = SettingsManager.SharerExtensionManager.Features.First();

            NewsSharerWindow sharerWindow =
                feature.Extension.CreateSharerWindow(article, feature.SettingsGroup.KeyValueStorage);
            sharerWindow.JobsCreated += SharerWindow_JobsCreated;
            sharerWindow.ShowDialog();
        }

        private void SharerWindow_JobsCreated(object sender, JobsCreatedEventArgs e)
        {
            foreach(IShareJob job in e.Jobs)
            {
                job.Finished += Job_Finished;
            }
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.Source.GetType());
            System.Diagnostics.Debug.WriteLine(e.OriginalSource.GetType());

            foreach(NewsArticle newsArticle in SelectedNewsArticles)
            {
                System.Diagnostics.Debug.WriteLine(newsArticle.Title);
            }

        }

        private void SkipJobButton_Click(object sender, RoutedEventArgs e)
        {
            // remove the job from the extension's KVS
            IShareJob currentJob = ShareJobQueue.CurrentJob;
            currentJob?.Skip();

            // continue on with the queue
            ShareJobQueue.RunAllJobs();
        }
    }
}
