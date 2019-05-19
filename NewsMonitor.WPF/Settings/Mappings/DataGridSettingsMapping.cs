using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NewsMonitor.WPF.Settings.Mappings
{
    public class DataGridSettingsMapping : SettingsMapping
    {
        DataGrid _dataGrid;
        IListSerializer _serializer;

        public DataGridSettingsMapping(string storageKey, string defaultValue,
            DataGrid dataGrid, IListSerializer serializer) : 
            base(storageKey, defaultValue)
        {
            _dataGrid = dataGrid;
            _serializer = serializer;
        }

        public override void Deserialize(string val)
        {
            _dataGrid.ItemsSource = _serializer.Deserialize(val);
        }

        public override string Serialize()
        {
            return _serializer.Serialize(_dataGrid.Items);
        }
    }
}
