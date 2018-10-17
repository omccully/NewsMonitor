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

        public MainWindow()
        {
            InitializeComponent();

            dbContext = new DatabaseContext("NewsMonitorDb");

            try
            {
                this.SettingsManager = new SettingsManager(
                    new DatabaseKeyValueStorage(dbContext));
            }
            catch(InvalidExtensionTypeException e)
            {
                MessageBox.Show(e.Message);
                this.Close();
            }
            

            AllNewsArticles = new ObservableCollection<NewsArticle>(dbContext.NewsArticles.ToList());
            NewsArticleFilter = new ObservableCollectionFilter<NewsArticle>(AllNewsArticles);

            NewsArticlesDataGrid.ItemsSource = NewsArticleFilter.FilteredResults;
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

        private void FindArticlesButton_Click(object sender, RoutedEventArgs e)
        {
            FindArticlesButton.IsEnabled = false;

            IEnumerable<INewsSearcher> news_searchers = NewsSearchers;
            HashSet<string> existing_article_urls = ExistingArticleUrls;

            string searchTermsStr = SettingsManager.General
                .KeyValueStorage.GetString(GeneralSettingsPage.SearchTermsKey);
            var search_terms = searchTermsStr.Split(',').Select(str => str.Trim());

            foreach (string search_term in search_terms)
            {
                Console.WriteLine("Search Term=" + search_term);
                foreach(INewsSearcher news_searcher in news_searchers)
                {
                    Console.WriteLine("News Searcher=" + news_searcher.GetType());
                    var results = news_searcher.Search(search_term)
                        .Where(article => !existing_article_urls.Contains(article.Url));
                    foreach (NewsArticle article in results)
                    {
                        AllNewsArticles.Add(article);
                        Console.WriteLine(article);
                    }
                }
            }
            Console.WriteLine("finished");
            FindArticlesButton.IsEnabled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow sw = new SettingsWindow(SettingsManager.SettingsGroups);
            sw.ShowDialog();
        }
    }
}
