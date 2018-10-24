using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NewsMonitor.WPF.Views.EditableTreeView
{
    public class EditableTreeView : TreeView
    {
        public int MaxItemsForAutoExpand { get; set; } = 20;

        public EditableTreeView()
        {
            // allow for delete functionality
            KeyUp += EditableTreeView_KeyUp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="rules">The first item is the rules for level 1 of the tree.
        /// The second item is the rules for level 2 of the tree, etc </param>
        public void LoadModelWithRules(TreeModel<string> model, IEnumerable<EditableTreeViewLevelRule> rules)
        { 
            this.RestoreFromTreeModel(model);

            ExpandAll();

            AddTextBoxes(this, rules);
        }

        void ExpandAll()
        {
            foreach (object item in Items)
            {
                TreeViewItem tvi = item as TreeViewItem;
                if (tvi == null) continue;

                if(tvi.Items.Count <= MaxItemsForAutoExpand)
                {
                    tvi.IsExpanded = true;
                }
            }
        }

        #region Add
        TextBox AddTextBoxes(ItemsControl control, IEnumerable<EditableTreeViewLevelRule> rules)
        {
            EditableTreeViewLevelRule thisLayerRules = rules.First();

            TextBox tb = new TextBox()
            {
                MinWidth = 50,
                Text = thisLayerRules.HelpText
            };

            tb.GotKeyboardFocus += (o, e) =>
            {
                if (tb.Text == thisLayerRules.HelpText) tb.Text = "";
            };

            tb.LostKeyboardFocus += (o, e) =>
            {
                if (String.IsNullOrWhiteSpace(tb.Text)) tb.Text = thisLayerRules.HelpText;
            };

            tb.KeyUp += (o, e) =>
            {
                tb.Background = new SolidColorBrush(Colors.White);
                if (e.Key != Key.Enter) return;

                e.Handled = true;

                // enter button pressed

                string newItemText = tb.Text;

                if (thisLayerRules.Unique)
                {
                    if (ItemsControlHasChildWithHeader(control, newItemText))
                    {
                        tb.Background = new SolidColorBrush(Colors.Red);
                        return;
                    }
                }

                TreeViewItem tvi = new TreeViewItem()
                {
                    Header = newItemText
                };

                TextBox innerTb = null;
                if (rules.Count() > 1)
                {
                    tvi.IsExpanded = true;
                    innerTb = AddTextBoxes(tvi, rules.Skip(1));
                }
                tb.Text = "";
                control.Items.Insert(control.Items.Count - 1, tvi);

                // TODO: fix this. doesn't work
                // the keyboard focus should switch to the inner textbox
                // when a new section is added
                if(innerTb != null) Keyboard.Focus(innerTb);
            };

            if (rules.Count() > 1)
            {
                foreach (object item in control.Items)
                {
                    TreeViewItem tvi = item as TreeViewItem;
                    if (tvi == null) continue;
                    AddTextBoxes(tvi, rules.Skip(1));
                }
            }

            control.Items.Add(tb);
            return tb;
        }

        bool ItemsControlHasChildWithHeader(ItemsControl control, string testStr)
        {
            foreach (object item in control.Items)
            {
                TreeViewItem foo = item as TreeViewItem;

                if (foo != null && foo.Header.ToString() == testStr)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region Delete
        private void EditableTreeView_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != Key.Delete) return;

            e.Handled = true;

            DeleteFirstSelectedTreeViewItem(this, true);
        }

        bool DeleteFirstSelectedTreeViewItem(ItemsControl parent, bool recursive = false)
        {
            List<object> itemsCopy = new List<object>(parent.Items.Cast<object>());
            foreach (object item in itemsCopy)
            {
                TreeViewItem tvi = item as TreeViewItem;
                if (tvi == null) continue;

                if (recursive)
                {
                    if (DeleteFirstSelectedTreeViewItem(tvi, true))
                    {
                        return true;
                    }
                }

                if (tvi.IsSelected)
                {
                    Console.WriteLine("delete " + tvi.Header);
                    parent.Items.Remove(item);
                    return true;
                }
            }
            return false;
        }
        #endregion


        public TreeModel<string> ToModel()
        {
            throw new NotImplementedException();
        }
    }
}
