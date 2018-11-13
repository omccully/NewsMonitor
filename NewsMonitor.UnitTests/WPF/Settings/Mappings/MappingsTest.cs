using Microsoft.VisualStudio.TestTools.UnitTesting;
using NewsMonitor.WPF.Settings.Mappings;
using NewsMonitor.WPF.Views.EditableTreeView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.UnitTests.WPF.Settings.Mappings
{
    [TestClass]
    public class MappingsTest
    {
        [TestMethod]
        public void TestEditableTreeViewSettingsMapping()
        {
            TreeModel<string> tm = new TreeModel<string>("",
               new List<TreeModel<string>>()
               {
                    new TreeModel<string>("section 1",
                        new List<TreeModel<string>>()
                        {
                            new TreeModel<string>("sec1item1"),
                            new TreeModel<string>("sec1item2")
                        }),
                    new TreeModel<string>("section 2"),
               });

            List<EditableTreeViewLevelRule> rules =
                new List<EditableTreeViewLevelRule>()
            {
                new EditableTreeViewLevelRule("section", true),
                new EditableTreeViewLevelRule("item", true),

            };

            EditableTreeView etv = new EditableTreeView();
            //etv.LoadModelWithRules(tm, rules);

            EditableTreeViewSettingsMapping mapping =
                new EditableTreeViewSettingsMapping("test", null,
                etv, rules);

            string serialized = mapping.Serialize();

            EditableTreeView dupe = new EditableTreeView();
            EditableTreeViewSettingsMapping mapdupe =
                new EditableTreeViewSettingsMapping("test", null,
                etv, rules);
            mapdupe.Deserialize(serialized);

            Assert.AreEqual(etv.SaveToTreeModel(""), dupe.SaveToTreeModel(""));
            Assert.AreEqual(serialized, mapdupe.Serialize());
        }


        [TestMethod]
        public void TestTreeModelSettingsMapping ()
        {
            TreeModel<string> tm = new TreeModel<string>("",
               new List<TreeModel<string>>()
               {
                    new TreeModel<string>("section 1",
                        new List<TreeModel<string>>()
                        {
                            new TreeModel<string>("sec1item1"),
                            new TreeModel<string>("sec1item2")
                        }),
                    new TreeModel<string>("section 2"),
               });


            TreeModelSettingsMapping map =
                new TreeModelSettingsMapping("test", "", tm);

            string serialized = map.Serialize();

            TreeModel<string> dupe = new TreeModel<string>("");
            TreeModelSettingsMapping mapdupe =
                new TreeModelSettingsMapping("test", "", dupe);
            mapdupe.Deserialize(serialized);

            Assert.AreEqual(tm, dupe);
            Assert.AreEqual(serialized, mapdupe.Serialize());
        }

    }
}
