using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using NewsMonitor.Data.Models;

namespace NewsMonitor.Data.Database
{
    public class DatabaseKeyValueStorage : KeyValueStorage
    {
        DatabaseContext dbContext;

        public DatabaseKeyValueStorage(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public override string GetString(string key, string default_val = null)
        {
            KeyValuePair kvp = GetKvpForKey(key);
            return (kvp == null ? default_val : kvp.Value);
        }

        public override KeyValueStorage SetValue(string key, string val)
        {
            KeyValuePair kvp = GetKvpForKey(key);
            if(kvp != null)
            {
                kvp.Value = val;
            }
            else
            {
                dbContext.KeyValuePairs.Add(new KeyValuePair() { Key = key, Value = val });
            }
            
            dbContext.SaveChanges();
            return this;
        }

        protected KeyValuePair GetKvpForKey(string key)
        {
            var list = dbContext.KeyValuePairs.Where(kvp => kvp.Key == key).ToList();
            if (list.Count > 1)
                throw new Exception("Key appears in database multiple times");
            if (list.Count == 0) return null;
            return list.ElementAt(0);
        }
    }
}
