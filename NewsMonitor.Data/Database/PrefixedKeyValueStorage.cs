using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.Data.Database
{
    public class PrefixedKeyValueStorage : KeyValueStorage
    {
        KeyValueStorage InnerKvs;
        string Prefix;

        public PrefixedKeyValueStorage(KeyValueStorage innerKvs, string prefix)
        {
            this.InnerKvs = innerKvs;
            this.Prefix = prefix;
        }

        public override string GetString(string key, string default_val = null)
        {
            return InnerKvs.GetString(Prefix + key, default_val);
        }

        public override KeyValueStorage SetValue(string key, string val)
        {
            InnerKvs.SetValue(Prefix + key, val);
            return this;
        }

        // TODO: implement rest of these, otherwise all values will always
        // be stored as strings in InnerKvs
    }
}
