using NewsMonitor.Data.Models;
using NewsMonitor.Services;
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

namespace NewsMonitor.WPF.Views
{
    /// <summary>
    /// Interaction logic for ShareHistoryPage.xaml
    /// </summary>
    public partial class ShareHistoryPage : Page
    {
        public IUrlLauncher UrlLauncher { get; set; } = new UrlLauncher();

        ObservableCollection<ShareJobResult> AllShareJobResults;

        public ShareHistoryPage(ObservableCollection<ShareJobResult> allShareJobResults)
        {
            InitializeComponent();

            this.AllShareJobResults = allShareJobResults;
            ShareHistoryDataGrid.ItemsSource = allShareJobResults;
        }

        void OnHyperlinkClick(object sender, RoutedEventArgs e)
        {
            object dataContext = ((Hyperlink)e.OriginalSource).DataContext;

            ShareJobResult sjr = dataContext as ShareJobResult;
           
            try
            {
                if (sjr != null) UrlLauncher.Launch(sjr.Url);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }
    }
}
