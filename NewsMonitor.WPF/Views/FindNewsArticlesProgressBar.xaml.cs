using NewsMonitor.Data.Models;
using NewsMonitor.Services;
using NewsMonitor.Services.NewsSearchers;
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
    /// Interaction logic for FindNewsArticlesProgressBar.xaml
    /// </summary>
    public partial class FindNewsArticlesProgressBar : UserControl
    {
        public event EventHandler<NewsArticlesFoundEventArgs> ArticlesFound;

        public IEnumerable<INewsSearcher> NewsSearchers { get; set; }

        public Func<IEnumerable<string>> ProvideSearchTerms { get; set; }

        public IEnumerable<ExtensionFeature<INewsFilterExtension>> FilterFeatures { get; set; }

        public FindNewsArticlesProgressBar()
        {
            InitializeComponent();
        }

        public FindNewsArticlesProgressBar(
            IEnumerable<INewsSearcher> newsSearchers,
            Func<IEnumerable<string>> searchTermsProvider,
            IEnumerable<ExtensionFeature<INewsFilterExtension>> filterFeatures)
            : this()
        {
            this.NewsSearchers = newsSearchers;
            this.ProvideSearchTerms = searchTermsProvider;
            this.FilterFeatures = filterFeatures;
        }


        bool PassesAllFilters(NewsArticle newsArticle, string searchTerm)
        {
            return FilterFeatures.All(
                f => f.Extension.AllowArticle(newsArticle, searchTerm, f.KeyValueStorage));
        }

        public async Task FindArticlesAsync()
        {
            try
            {
                ProgressBar.Value = 0;
                await SearchForTermsWithNewsSearchers(NewsSearchers, ProvideSearchTerms());

                Console.WriteLine("finished");
            }
            finally
            {
                ProgressBar.Value = 0;
            }
        }

        async Task SearchForTermsWithNewsSearchers(IEnumerable<INewsSearcher> news_searchers, IEnumerable<string> search_terms)
        {
            ProgressBar.Maximum = news_searchers.Count() * search_terms.Count();

            foreach (string search_term in search_terms)
            {
                Console.WriteLine("Search Term=" + search_term);

                foreach (INewsSearcher news_searcher in news_searchers)
                {
                    Console.WriteLine("News Searcher=" + news_searcher.GetType());

                    await SearchForTermWithNewsSearcher(news_searcher, search_term);

                    ProgressBar.Value++;
                }
            }
        }

        async Task SearchForTermWithNewsSearcher(INewsSearcher news_searcher, string search_term)
        {
            IEnumerable<NewsArticle> results = (await news_searcher.SearchAsync(search_term))
                            .Where(article => PassesAllFilters(article, search_term));

            ArticlesFound?.Invoke(this, new NewsArticlesFoundEventArgs(results));
        }
    }
}
