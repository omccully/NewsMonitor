using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using NewsMonitor.WPF.Views.EditableTreeView;

namespace NewsMonitor.WPF.Settings.Mappings
{
    public class EditableTreeViewSettingsMapping : SettingsMapping
    {
        EditableTreeView TreeView;
        IEnumerable<EditableTreeViewLevelRule> Rules;

        TreeModel<string> Model;
        TreeModelSettingsMapping TreeModelMapping;

        public EditableTreeViewSettingsMapping(string storageKey,
             string defaultValue, EditableTreeView treeView,
             IEnumerable<EditableTreeViewLevelRule> rules)
             : base(storageKey, defaultValue)
        {
            this.TreeView = treeView;
            this.Rules = rules;

            this.Model = new TreeModel<string>("");
            this.TreeModelMapping = new TreeModelSettingsMapping(
                storageKey, defaultValue, this.Model);
        }

        public override void Deserialize(string val)
        {
            Console.WriteLine(val);

            // sets Model
            TreeModelMapping.Deserialize(val);

            if (Model == null)
            {
                TreeView.Items.Clear();
            }
            else
            {
                TreeView.Items.Clear();
                TreeView.LoadModelWithRules(Model, Rules);
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
