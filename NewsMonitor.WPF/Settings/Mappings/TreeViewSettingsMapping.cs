using NewsMonitor.WPF.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Serialization;
using NewsMonitor.WPF.Views.EditableTreeView;

namespace NewsMonitor.WPF.Settings.Mappings
{
    public class TreeViewSettingsMapping : SettingsMapping
    {
        TreeView TreeView;

        TreeModel<string> Model;
        TreeModelSettingsMapping TreeModelMapping;

        public TreeViewSettingsMapping(string storageKey, 
             string defaultValue, TreeView treeView)
             : base(storageKey, defaultValue)
        {
            this.TreeView = treeView;
            this.Model = new TreeModel<string>("");
            this.TreeModelMapping = new TreeModelSettingsMapping(
                storageKey, defaultValue, this.Model);
        }

        public override void Deserialize(string val)
        {
            // XML deserialize into TreeModel<string>
            // convert the TreeModel<string> to the treeView

            TreeModelMapping.Deserialize(val);

            //Console.WriteLine(val);

            if(Model == null)
            {
                TreeView.Items.Clear();
            }
            else
            {
                TreeView.Items.Clear();
                TreeView.RestoreFromTreeModel(Model);
            }
        }

        public override string Serialize()
        {
            // convert treeview into TreeModel<string>
            // serialize TreeModel<string> 

            Model.Children = TreeView.SaveToTreeModel();

            return TreeModelMapping.Serialize();
        }
    }
}
