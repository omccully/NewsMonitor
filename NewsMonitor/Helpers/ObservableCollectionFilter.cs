using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.Helpers
{
    public class ObservableCollectionFilter<T>
    {
        ReadOnlyObservableCollection<T> SourceData;

        public ReadOnlyObservableCollection<T> FilteredResults;
        ObservableCollection<T> EditableFilteredResults;

        Func<T, bool> _Filter = (item) => true;
        public Func<T, bool> Filter
        {
            get
            {
                return _Filter;
            }
            set
            {
                _Filter = value;
                Refresh();
            }
        }

        public ObservableCollectionFilter(ObservableCollection<T> source_data,
            Func<T, bool> filter = null)
            : this(new ReadOnlyObservableCollection<T>(source_data), filter)
        {

        }

        public ObservableCollectionFilter(ReadOnlyObservableCollection<T> source_data,
            Func<T, bool> filter = null)
        {
            SourceData = source_data;
            foreach (T item in SourceData)
            {
                INotifyPropertyChanged npc = item as INotifyPropertyChanged;

                if (npc != null)
                {
                    npc.PropertyChanged -= Npc_PropertyChanged;
                    npc.PropertyChanged += Npc_PropertyChanged;
                }
            }

            ((INotifyCollectionChanged)source_data).CollectionChanged += SourceData_CollectionChanged;

            if (filter != null) this._Filter = filter;

            EditableFilteredResults = new ObservableCollection<T>(source_data.Where(_Filter));

            FilteredResults = new ReadOnlyObservableCollection<T>(EditableFilteredResults);
        }

        private void SourceData_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // don't Refresh() if none of the added or removed elements are allowed by filter
            if (!AnyAllowed(e.NewItems) && !AnyAllowed(e.OldItems)) return;

            Refresh();
        }

        bool AnyAllowed(System.Collections.IList items)
        {
            if (items == null) return false;
            bool any_showing = false;
            foreach (T item in items)
            {
                if (Filter(item)) any_showing = true;
            }
            return any_showing;
        }

        void Refresh()
        {
            EditableFilteredResults.Clear();
            foreach (T item in SourceData)
            {
                if (Filter(item))
                {
                    EditableFilteredResults.Add(item);
                }

                INotifyPropertyChanged npc = item as INotifyPropertyChanged;

                if (npc != null)
                {
                    npc.PropertyChanged -= Npc_PropertyChanged;
                    npc.PropertyChanged += Npc_PropertyChanged;
                }
            }
        }

        private void Npc_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ObservableCollectionFilter.Npc_PropertyChanged");
            Refresh();
        }
    }
}
