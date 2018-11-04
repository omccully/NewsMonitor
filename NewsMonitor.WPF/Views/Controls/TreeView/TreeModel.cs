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

        public Dictionary<T, IEnumerable<T>> GetNextLayerDictionary()
        {
            Dictionary<T, IEnumerable<T>> dict = new Dictionary<T, IEnumerable<T>>();

            foreach(TreeModel<T> child in Children)
            {
                dict.Add(child.NodeValue,
                    child.Children.Select(c => c.NodeValue).ToList());
            }

            return dict;
        }

        public static Dictionary<T, IEnumerable<T>> LayerToDictionary(IEnumerable<TreeModel<T>> treeModels)
        {
            return treeModels.ToDictionary(tm => tm.NodeValue, 
                tm => tm.Children.Select(tmInner => tmInner.NodeValue));
        }
    }
}
