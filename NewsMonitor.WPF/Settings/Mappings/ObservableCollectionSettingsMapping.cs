using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NewsMonitor.WPF.Settings.Mappings
{
    public class ObservableCollectionSettingsMapping : SettingsMapping
    {
        ObservableCollection<string> Items;

        public ObservableCollectionSettingsMapping(string storageKey,
             string defaultValue, ObservableCollection<string> items)
             : base(storageKey, defaultValue)
        {
            this.Items = items;
        }

        public override void Deserialize(string val)
        {
            Items.Clear();
            if (val != null)
            {
                XmlSerializer reader = new XmlSerializer(typeof(string[]));
                var arr = (string[])reader.Deserialize(new StringReader(val));
                foreach(string s in arr)
                {
                    Items.Add(s);
                }
            }
        }

        public override string Serialize()
        {
            XmlSerializer writer = new XmlSerializer(typeof(string[]));
            StringWriter sw = new StringWriter();

            writer.Serialize(sw, Items.ToArray());
            string result = sw.ToString();
            Console.WriteLine(result);
            return result;
        }
    }
}
