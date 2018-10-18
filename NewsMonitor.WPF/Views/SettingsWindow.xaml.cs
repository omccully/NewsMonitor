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
using NewsMonitor.WPF.Settings;
using NewsMonitor.WPF.Views;

namespace NewsMonitor.WPF
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(IEnumerable<ISettingsGroup> settingsGroups)
        {
            InitializeComponent();

            // generate the GUI based on the SettingsGroups
            AddSettingsGroupsToItemControl(SettingsPageSelectorTreeView, settingsGroups);
        }

        bool first = true;
        void AddSettingsGroupsToItemControl(ItemsControl control, IEnumerable<ISettingsGroup> settingsGroups)
        {
            foreach (ISettingsGroup settingsGroup in settingsGroups)
            {
                var tvi = new TreeViewItem()
                {
                    Header = settingsGroup.Name,
                    IsExpanded = true
                };

                tvi.Selected += delegate (object sender, RoutedEventArgs e)
                {
                    Console.WriteLine("Selected " + settingsGroup.Name);
                    SelectedSettingsGroup = settingsGroup;

                    e.Handled = true;
                };

                if (first)
                {
                    // select the first TreeViewItem
                    // default to general settings
                    tvi.IsSelected = true;
                    first = false;
                }

                if (settingsGroup.Children != null && 
                    settingsGroup.Children.Count() > 0)
                {
                    AddSettingsGroupsToItemControl(tvi, settingsGroup.Children);
                } 

                control.Items.Add(tvi);
            }
        }

        ISettingsGroup _SelectedSettingsGroup;
        ISettingsGroup SelectedSettingsGroup
        {
            get
            {
                return _SelectedSettingsGroup;
            }
            set
            {
                _SelectedSettingsGroup = value;
                CurrentSettingsPage = value.CreateSettingsPageWithStorage();
            }
        }

        SettingsPage _CurrentSettingsPage;
        SettingsPage CurrentSettingsPage
        {
            get
            {
                return _CurrentSettingsPage;
            }
            set
            {
                _CurrentSettingsPage?.Save();

                _CurrentSettingsPage = value;

                if(value == null)
                {
                    SettingsFrame.Navigate(new EmptySettingsPage());
                }
                else
                {
                    SettingsFrame.Navigate(value);
                    value.Restore();
                }
               

                value?.Restore();
            }
        }

        void SaveCurrentPage()
        {
            CurrentSettingsPage?.Save();
        }

        void RestoreCurrentPage()
        {
            CurrentSettingsPage?.Restore();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentPage();
            this.Close();
        }
    }
}
