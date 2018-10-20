using NewsMonitor.Data.Database;
using NewsMonitor.WPF.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NewsMonitor.WPF.Views
{
    public abstract class SettingsPage : Page
    {
        public KeyValueStorage KeyValueStorage { get; set; }

        protected List<SettingsMapping> SettingsMappings { get; } = new List<SettingsMapping>();

        public virtual void Restore()
        {
            // restore from SettingsMappings
            foreach(SettingsMapping mapping in SettingsMappings)
            {
                string savedVal = KeyValueStorage.GetString(mapping.StorageKey, mapping.DefaultValue);
                mapping.TextBox.Text = savedVal;
                Console.WriteLine($"{mapping.StorageKey} = {savedVal} (default {mapping.DefaultValue})");
            }
        }

        public virtual void Save()
        {
            // Save to SettingsMappings

            foreach (SettingsMapping mapping in SettingsMappings)
            { 
                if(!mapping.IsValid)
                {
                    MessageBox.Show($"\"{mapping.TextBox.Text}\" is invalid input for {mapping.TextBox.Name}:" 
                        + Environment.NewLine + mapping.ErrorMessage);
                    return;
                }
            }

            foreach (SettingsMapping mapping in SettingsMappings)
            {
                KeyValueStorage.SetValue(mapping.StorageKey, mapping.TextBox.Text);
            }
        }
    }
}
