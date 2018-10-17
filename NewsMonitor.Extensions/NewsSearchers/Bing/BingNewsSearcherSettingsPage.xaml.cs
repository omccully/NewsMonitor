using NewsMonitor.WPF.Views;
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

namespace NewsMonitor.Extensions.NewsSearchers.Bing
{
    /// <summary>
    /// Interaction logic for BingNewsSettingsPage.xaml
    /// </summary>
    public partial class BingNewsSearcherSettingsPage : SettingsPage
    {
        public BingNewsSearcherSettingsPage()
        {
            InitializeComponent();
            Console.WriteLine("BingNewsSearcherSettingsPage");
        }

        public const string BingNewsAccessKeyStorageKey = "BingNewsAccessKey";

        public override void Restore()
        {
            BingNewsAccessKeyTextBox.Text = KeyValueStorage.GetString(BingNewsAccessKeyStorageKey, "");
        }

        public override void Save()
        {
            KeyValueStorage.SetValue(BingNewsAccessKeyStorageKey, BingNewsAccessKeyTextBox.Text);
        }
    }
}
