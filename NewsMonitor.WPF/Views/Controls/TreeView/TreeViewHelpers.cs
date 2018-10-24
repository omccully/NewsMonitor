using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NewsMonitor.WPF.Views.EditableTreeView
{
    public static class TreeViewHelpers
    {
        public static void RestoreFromTreeModel(this ItemsControl control, TreeModel<string> model)
        {
            if (control.HasItems) control.Items.Clear();

            foreach (TreeModel<string> innerModel in model.Children)
            {
                TreeViewItem tvi = new TreeViewItem()
                {
                    Header = innerModel.NodeValue
                };

                if (model.Children != null && model.Children.Count > 0)
                {
                    RestoreFromTreeModel(tvi, innerModel);
                }

                control.Items.Add(tvi);
            }
        }

        public static List<TreeModel<string>> SaveToTreeModel(this ItemsControl control)
        {
            List<TreeModel<string>> outerModels = new List<TreeModel<string>>();

            foreach (object item in control.Items)
            {
                TreeViewItem tvi = item as TreeViewItem;
                if (tvi == null) continue;

                // if has children, get all children with SaveToTreeModel(tvi)
                // if no children, use null to specify no children
                outerModels.Add(new TreeModel<string>(tvi.Header.ToString(),
                    (tvi.Items.Count > 0) ? SaveToTreeModel(tvi) : null
                ));
            }

            return outerModels;
        }

        public static void ExpandAll(this TreeViewItem tvi, int childLimit=0, bool recursive=false)
        {
            ExpandAll((ItemsControl)tvi, childLimit, recursive);
        }

        public static void ExpandAll(this TreeView tv, int childLimit=0, bool recursive = false)
        {
            ExpandAll((ItemsControl)tv, childLimit, recursive);
        }

        static void ExpandAll(ItemsControl control, int childLimit=0, bool recursive = false)
        {
            foreach (object item in control.Items)
            {
                TreeViewItem tvi = item as TreeViewItem;
                if (tvi == null) continue;

                if (childLimit == 0 || tvi.Items.Count <= childLimit)
                {
                    tvi.IsExpanded = true;
                    if(recursive) ExpandAll((ItemsControl)tvi, childLimit, recursive);
                }
            }
        }
    }
}
