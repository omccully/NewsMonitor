using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsMonitor.WPF.Views.EditableTreeView;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace NewsMonitor.UnitTests.WPF.Views.Controls.TreeView
{
    [TestClass]
    public class EditableTreeViewTest
    {

        [TestMethod]
        public void LoadModelWithRules()
        {
            EditableTreeView editableTv = new EditableTreeView();

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

            editableTv.LoadModelWithRules(tm, rules);

            TreeModel<string> saved = new TreeModel<string>("", editableTv.SaveToTreeModel());
            Assert.AreEqual(tm, saved);

            Assert.IsTrue(LayersHaveTextBoxes(editableTv, 2));
        }

        bool LayersHaveTextBoxes(ItemsControl control, int layers)
        {
            if (layers < 1) return true;
            if (layers == 1) return control.Items.Cast<object>().Last() is TextBox;
            
            foreach(object item in control.Items)
            {
                ItemsControl innerControl = item as ItemsControl;
                if (innerControl == null) continue;

                if (!LayersHaveTextBoxes(innerControl, layers - 1)) return false;
            }

            return true;
        }

        [TestMethod]
        public void AddWithTextbox()
        {
            EditableTreeView editableTv = new EditableTreeView();

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

            editableTv.LoadModelWithRules(tm, rules);

            foreach(object item in editableTv.Items)
            {
                if(item is TextBox)
                {
                    TextBox tb = (TextBox)item;
                    tb.Text = "newitem";

                    tb.RaiseEvent(new KeyEventArgs(
                        Keyboard.PrimaryDevice,
                        new FakePresentationSource(),
                        0,
                        Key.Enter)
                    {
                        RoutedEvent = Keyboard.KeyUpEvent
                    });

                    break;
                }
            }

            TreeModel<string> saved = new TreeModel<string>("", editableTv.SaveToTreeModel());

            TreeModel<string> expectedResult = new TreeModel<string>("",
                new List<TreeModel<string>>()
                {
                    new TreeModel<string>("section 1",
                        new List<TreeModel<string>>()
                        {
                            new TreeModel<string>("sec1item1"),
                            new TreeModel<string>("sec1item2")
                        }),
                    new TreeModel<string>("section 2"),
                    new TreeModel<string>("newitem")
                });

            Assert.AreEqual(expectedResult, saved);

            Assert.IsTrue(LayersHaveTextBoxes(editableTv, 2));
        }

        [TestMethod]
        public void ExtensionMethods()
        {
            System.Windows.Controls.TreeView tv = 
                new System.Windows.Controls.TreeView();

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

            tv.RestoreFromTreeModel(tm);

            TreeModel<string> result = tv.SaveToTreeModel("");

            Assert.AreEqual(tm, result);
        }
    }
}
