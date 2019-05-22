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

namespace NewsMonitor.WPF.Views
{
    /// <summary>
    /// Interaction logic for UrlSelectorWindow.xaml
    /// </summary>
    public partial class UrlSelectorWindow : Window
    {
        public UrlSelectorWindow(IEnumerable<string> urls)
        {
            InitializeComponent();

            UrlSelectorItemsControl.ItemsSource = urls;
        }

        public string SelectedUrl { get; private set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SelectedUrl = ((Button)sender).Content.ToString();
            this.Close();
        }
    }
}
