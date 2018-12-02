using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace NewsMonitor.Helpers
{
    /// <summary>
    /// HashSet wrapper for an ObservableCollection to provide quick checking for the existence of items. 
    /// </summary>
    /// <typeparam name="TItem">Type of items in the collection</typeparam>
    /// <typeparam name="T">Type of items in the underlying hashset</typeparam>
    public class ObservableCollectionSearcher<TItem, T>
    {
        HashSet<T> InnerHashSet;
        object InnerHashSetLock = new object();
        ReadOnlyObservableCollection<TItem> Collection;
        Func<TItem, T> TransformTItemToT;

        public ObservableCollectionSearcher(ObservableCollection<TItem> collection, Func<TItem, T> selector)
            : this(new ReadOnlyObservableCollection<TItem>(collection), selector)
        { 
        }

        public ObservableCollectionSearcher(ReadOnlyObservableCollection<TItem> collection, Func<TItem, T> selector)
        {
            this.Collection = collection;
            this.TransformTItemToT = selector;

            ((INotifyCollectionChanged)collection).CollectionChanged += Collection_CollectionChanged;
        }

        private void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            lock (InnerHashSetLock)
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    if(e.NewItems != null && InnerHashSet != null)
                    {
                        foreach(TItem item in e.NewItems)
                        {
                            InnerHashSet.Add(TransformTItemToT(item));
                        }
                    }
                    return;
                }

                InnerHashSet = null; // invalidate InnerHashSet
            }
        }

        void InitializeHashSet()
        {
            InnerHashSet = new HashSet<T>(Collection.Select(item => TransformTItemToT(item)));
        }

        public bool Contains(T item)
        {
            lock (InnerHashSetLock)
            {
                if (InnerHashSet == null) InitializeHashSet();
                return InnerHashSet.Contains(item);
            }
        }
    }
}
