using NewsMonitor.Data.Models;
using NewsMonitor.WPF.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using NewsMonitor.Services;
using NewsMonitor.Helpers;

namespace NewsMonitor.WPF.Views
{
    /// <summary>
    /// Interaction logic for NewsArticlesPage.xaml
    /// </summary>
    public partial class NewsArticlesPage : Page
    {
        public event EventHandler<JobsCreatedEventArgs> ShareJobsCreated;
        public event EventHandler NewsArticleModified;
        public event EventHandler QuickFilterFinished;

        public IUrlLauncher UrlLauncher { get; set; } = new UrlLauncher();

        IEnumerable<ExtensionFeature<INewsFilterExtension>> NewsFilterExtensionFeatures;
        IEnumerable<ExtensionFeature<INewsSharerExtension>> NewsSharerExtensionFeatures;

        ObservableCollectionFilter<NewsArticle> NewsArticleFilter;
        ReadOnlyObservableCollection<NewsArticle> NewsArticlesSource;

        public NewsArticlesPage(ObservableCollection<NewsArticle> allNewsArticles, 
            IEnumerable<ExtensionFeature<INewsFilterExtension>> newsFilterExtensionFeatures,
            IEnumerable<ExtensionFeature<INewsSharerExtension>> newsSharerExtensionFeatures)
        {
            InitializeComponent();

            NewsArticleFilter = new ObservableCollectionFilter<NewsArticle>(allNewsArticles,
                (na) => !na.Hidden);
            NewsArticlesSource = NewsArticleFilter.FilteredResults;
            NewsArticlesDataGrid.ItemsSource = NewsArticleFilter.FilteredResults;

            this.NewsFilterExtensionFeatures = newsFilterExtensionFeatures;
            this.NewsSharerExtensionFeatures = newsSharerExtensionFeatures;

            InitializeDataGridRowContextMenuAll();
        }

        public IEnumerable<NewsArticle> SelectedNewsArticles
        {
            get
            {
                return NewsArticlesDataGrid.SelectedItems.Cast<NewsArticle>().ToList();
            }
        }

        void InitializeDataGridRowContextMenu<T>(IEnumerable<ExtensionFeature<T>> features, string menuItemName, 
            Action<NewsArticle, ExtensionFeature<T>> selectAction) where T : ISettingsGroupExtension
        {
            ContextMenu cm = (ContextMenu)NewsArticlesDataGrid.FindResource("RowMenu");

            MenuItem fmi = cm.Items.Cast<MenuItem>().Where(mi => mi.Name == menuItemName).First();
            foreach (ExtensionFeature<T> feature in features)
            {
                MenuItem mi = new MenuItem() { Header = feature.Name };
                mi.Click += (o, e) =>
                {
                    foreach (NewsArticle article in SelectedNewsArticles)
                    {
                        System.Diagnostics.Debug.WriteLine(article.Title);
                        selectAction(article, feature);
                    }

                    e.Handled = true;
                };
                fmi.Items.Add(mi);
            }
        }

        void InitializeDataGridRowContextMenuAll()
        {
            InitializeDataGridRowContextMenu(NewsFilterExtensionFeatures, "FilterMenuItem", FilterNewsArticle);
            InitializeDataGridRowContextMenu(NewsSharerExtensionFeatures, "ShareMenuItem", ShareNewsArticle);
        }

        


        #region Share
        private void ShareSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Share selected");

            foreach (NewsArticle article in SelectedNewsArticles)
            {
                ShareNewsArticle(article);
            }
        }

        void ShareNewsArticle(NewsArticle article, ExtensionFeature<INewsSharerExtension> feature = null)
        {
            if (feature == null) feature = NewsSharerExtensionFeatures.First();

            NewsSharerWindow sharerWindow =
                feature.Extension.CreateSharerWindow(article, feature.KeyValueStorage);
            sharerWindow.JobsCreated += SharerWindow_JobsCreated;
            sharerWindow.ShowDialog();
        }

        private void SharerWindow_JobsCreated(object sender, JobsCreatedEventArgs e)
        {
            ShareJobsCreated?.Invoke(this, e);
        }

        private void DataGridRow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.OriginalSource.GetType());
            System.Diagnostics.Debug.WriteLine(e.Source.GetType());

            NewsArticle article = ((DataGridRow)e.Source).Item as NewsArticle;
            ShareNewsArticle(article);
        }
        #endregion

        void OnHyperlinkClick(object sender, RoutedEventArgs e)
        {
            object dataContext = ((Hyperlink)e.OriginalSource).DataContext;

            NewsArticle article = dataContext as NewsArticle;

            try
            {
                if (article != null) UrlLauncher.Launch(article.Url);
            }
            catch(Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        #region Delete
        private void DeleteSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelectedNewsArticles();
        }

        void FilterNewsArticle(NewsArticle newsArticle, ExtensionFeature<INewsFilterExtension> feature = null)
        {
            if (feature == null) feature = NewsFilterExtensionFeatures.First();

            Window window = feature.Extension.CreateQuickFilterWindow(newsArticle, feature.KeyValueStorage);
            System.Diagnostics.Debug.WriteLine("window = " + window);
            if (window != null) window.ShowDialog();
            QuickFilterFinished?.Invoke(this, new EventArgs());
        }

        private void NewsArticlesDataGrid_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Delete) return;

            DeleteSelectedNewsArticles();
        }

        void DeleteSelectedNewsArticles()
        {
            int count = 0;
            foreach (NewsArticle article in SelectedNewsArticles)
            {
                article.Hidden = true;

                count++;
            }

            if (count > 0) NewsArticleModified?.Invoke(this, new EventArgs());
        }

        #endregion
    }
}
