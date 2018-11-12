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

        public TreeModel<T> Duplicate()
        {
            return new TreeModel<T>(this.NodeValue,
                this.Children.Select(tm => tm.Duplicate()).ToList());
        }

        public List<TreeModel<T>> Flatten(List<TreeModel<T>> result = null)
        {
            if(result == null) result = new List<TreeModel<T>>();

            result.Add(this);

            foreach(TreeModel<T> child in Children)
            {
                child.Flatten(result);
            }

            return result;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            TreeModel<T> tm = obj as TreeModel<T>;
            if (tm == null) return false;

            return Equals(tm);
        }

        public bool Equals(TreeModel<T> tm)
        {
            if (tm == null) return false;

            if(!Object.ReferenceEquals(tm.NodeValue, this.NodeValue))
            {
                if (tm.NodeValue == null)
                    return false; // tm.NodeValue is null but now this.NodeValue

                if (!tm.NodeValue.Equals(this.NodeValue)) return false;
            }

            if (tm.Children.Count != this.Children.Count) return false;

            int count = tm.Children.Count;
            for (int i = 0; i < count; i++)
            {
                // recursive call
                if(!tm.Children[i].Equals(this.Children[i])) return false;
            }

            return true;
        }



        // override object.GetHashCode
        public override int GetHashCode()
        {
            return NodeValue.GetHashCode() ^ Children.GetHashCode();
        }
    }
}
