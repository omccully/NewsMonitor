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
        IEnumerable<ISettingsGroup> SettingsGroups;

        public SettingsWindow(IEnumerable<ISettingsGroup> settingsGroups)
        {
            InitializeComponent();
            this.SettingsGroups = settingsGroups;

            // generate the GUI based on the SettingsGroup
            AddSettingsGroupsToItemControl(SettingsPageSelectorTreeView, settingsGroups);
        }

        void AddSettingsGroupsToItemControl(ItemsControl control, IEnumerable<ISettingsGroup> settingsGroups)
        {
            foreach (ISettingsGroup settingsGroup in settingsGroups)
            {
                var tvi = new TreeViewItem()
                {
                    Header = settingsGroup.Name,
                    IsExpanded = true
                };

                tvi.Selected += delegate
                {
                    Console.WriteLine("tvi.Selected");
                    Console.WriteLine(settingsGroup.Name);
                    SelectedSettingsGroup = settingsGroup;
                };

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


        private void GeneralSettingsTreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("GeneralSettingsTreeViewItem_Selected");
            CurrentSettingsPage = new GeneralSettingsPage();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentPage();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentPage();
            this.Close();
        }
    }
}
