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
using NewsMonitor.WPF.Settings.Mappings;
using NewsMonitor.WPF.Views.EditableTreeView;
using Microsoft.Win32;
using System.IO;

namespace NewsMonitor.Extensions.NewsFilters.RegexTitle
{
    /// <summary>
    /// Interaction logic for RegexTitleNewsFilterSettingsPage.xaml
    /// </summary>
    public partial class RegexTitleNewsFilterSettingsPage : SettingsPage
    {
        List<EditableTreeViewLevelRule> Rules = new List<EditableTreeViewLevelRule>()
        {
            new EditableTreeViewLevelRule("<search term or \"any\">"),
            new EditableTreeViewLevelRule("<regex or string>")
        };

        IStringSectionsSerializer Serializer = new JsonStringSectionSerializer();

        public RegexTitleNewsFilterSettingsPage()
        {
            InitializeComponent();

            SettingsMappings.Add(
                new EditableTreeViewSettingsMapping(RegexTitleTreeKey, null, RegexTitleTreeView, Rules));
        }

        public const string RegexTitleTreeKey = "RegexTitleTree";

        private void AddFromJsonFile_Click(object sender, RoutedEventArgs e)
        {
            PromptUserToFindFileToAdd(false);
        }

        private void ReplaceWithJsonFile_Click(object sender, RoutedEventArgs e)
        {
            PromptUserToFindFileToAdd(true);
        }

        void PromptUserToFindFileToAdd(bool deleteExisting=false)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".json"; 
            dlg.Filter = "JSON files (.json)|*.json"; 
            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                string fileName = dlg.FileName;

                AddFromFile(fileName, deleteExisting);
            }
        }

        private void SaveToJsonFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();

            dlg.DefaultExt = ".json"; // Default file extension
            dlg.Filter = "JSON files (.json)|*.json"; // Filter files by extension
            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                string fileName = dlg.FileName;
                
                Dictionary<string, IEnumerable<string>> dict = 
                    TreeModel<string>.LayerToDictionary(RegexTitleTreeView.SaveToTreeModel());

                File.WriteAllText(fileName, Serializer.Serialize(dict));
            }
        }

        void AddFromFile(string fileName, bool deleteExisting=false)
        {
            Dictionary<string, IEnumerable<string>> dict =
                Serializer.Deserialize(File.ReadAllText(fileName));

            if(deleteExisting)
            {
                RegexTitleTreeView.Items.Clear();
            }

            AddFromDictionary(dict);
        }

        void AddFromDictionary(Dictionary<string, IEnumerable<string>> dict)
        {
            foreach (KeyValuePair<string, IEnumerable<string>> kvp in dict)
            {
                TreeViewItem section = RegexTitleTreeView.TreeViewItemWithHeader(kvp.Key);

                if (section == null)
                {
                    section = new TreeViewItem() { Header = kvp.Key };
                    RegexTitleTreeView.Items.Add(section);
                    System.Diagnostics.Debug.WriteLine($"AddingSection {kvp.Key}");
                }

                foreach (string s in kvp.Value)
                {
                    if (!section.ContainsChildHeader(s))
                    {
                        System.Diagnostics.Debug.WriteLine($"Adding {kvp.Key}/{s}");
                        section.Items.Add(new TreeViewItem() { Header = s });
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Skip {kvp.Key}/{s}");
                    }
                }
            }

            RegexTitleTreeView.RefreshTextBoxes();
        }
    }
}
