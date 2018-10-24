using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.WPF.Views.EditableTreeView
{
    [Serializable]
    public class TreeModel<T>
    {
        public T NodeValue;
        public List<TreeModel<T>> Children;

        public TreeModel(T nodeValue, List<TreeModel<T>> leaves = null)
        {
            this.NodeValue = nodeValue;
            this.Children = leaves ?? new List<TreeModel<T>>();
        }

        public TreeModel()
        {

        }
    }
}
