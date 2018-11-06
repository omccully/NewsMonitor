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
using NewsMonitor.Data.Database;
using NewsMonitor.Data.Models;
using NewsMonitor.WPF.Views;
using NewsMonitor.WPF.Extensions;
using NewsMonitor.WPF.Views.EditableTreeView;
using NewsMonitor.WPF.Settings.Mappings;
using System.Collections.ObjectModel;

namespace NewsMonitor.Extensions.NewsFilters.RegexTitle
{
    public partial class RegexTitleQuickFilterWindow : Window
    {
        TreeModel<string> Model;

        public RegexTitleQuickFilterWindow(string title, TreeModel<string> model)
        {
            InitializeComponent();

            this.Model = model;
            SearchTermsComboBox.ItemsSource = SearchTerms;
            SearchTermsComboBox.SelectedIndex = 0;
            PostTitleTextBox.Text = title;
        }

        IEnumerable<string> SearchTerms
        {
            get
            {
                return Model.Children.Select(tm => tm.NodeValue);
            }
        }

        private void AddFilterButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("AddFilterButton_Click");
            string selectedSearchTerm = SearchTermsComboBox.SelectedItem.ToString();
            if (String.IsNullOrWhiteSpace(selectedSearchTerm))
            {
                System.Diagnostics.Debug.WriteLine("Selected search term is empty");
                MessageBox.Show("Selected search term is empty");
                return;
            }
            if (String.IsNullOrWhiteSpace(FilterTextOrRegex.Text))
            {
                System.Diagnostics.Debug.WriteLine("Filter is empty");
                MessageBox.Show("Filter is empty");
                return;
            }

            foreach (TreeModel<string> child in Model.Children)
            {
                if(child.NodeValue == selectedSearchTerm)
                {
                    if (child.Children.Any(tm => tm.NodeValue.ToLower() == FilterTextOrRegex.Text.ToLower()))
                    {
                        // already exists
                        System.Diagnostics.Debug.WriteLine("This filter already exists");
                        MessageBox.Show("This filter already exists");
                        return;
                    }

                    child.Children.Add(new TreeModel<string>(FilterTextOrRegex.Text));
                    this.Close();
                    OnFinished();
                    return;
                }
            }

            Model.Children.Add(new TreeModel<string>(selectedSearchTerm, new List<TreeModel<string>>()
            {
                new TreeModel<string>(FilterTextOrRegex.Text)
            }));
            this.Close();
            OnFinished();
        }

        public event EventHandler Finished;
        protected void OnFinished()
        {
            Finished?.Invoke(this, new EventArgs());
        }
    }
}
