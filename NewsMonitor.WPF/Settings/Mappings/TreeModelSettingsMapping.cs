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
    public class TreeModelSettingsMapping : SettingsMapping
    {
        TreeModel<string> TreeModel;

        public TreeModelSettingsMapping(string storageKey,
             string defaultValue, TreeModel<string> treeModel)
             : base(storageKey, defaultValue)
        {
            this.TreeModel = treeModel;
        }

        public override void Deserialize(string val)
        {
            if (val != null)
            {
                XmlSerializer reader = new XmlSerializer(typeof(TreeModel<string>));
                var tm = (TreeModel<string>)reader.Deserialize(new StringReader(val));
                TreeModel.Children = tm.Children;
            }
            else
            {
                TreeModel.Children = new List<TreeModel<string>>();
            }
        }

        public override string Serialize()
        {
            XmlSerializer writer = new XmlSerializer(typeof(TreeModel<string>));
            StringWriter sw = new StringWriter();

            writer.Serialize(sw, TreeModel);
            string result = sw.ToString();
            Console.WriteLine(result);
            return result;
        }
    }
}
