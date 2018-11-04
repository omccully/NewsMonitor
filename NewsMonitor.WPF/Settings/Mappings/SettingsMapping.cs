using NewsMonitor.Data.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NewsMonitor.WPF.Settings
{
    public abstract class SettingsMapping
    {
        public string StorageKey { get; private set; }
        public string DefaultValue { get; private set; }
        public TextValidator TextValidator { get; private set; }

        public SettingsMapping(string storageKey, string defaultValue, 
            TextValidator textValidator = null)
        {
            this.StorageKey = storageKey;
            this.DefaultValue = defaultValue;
            this.TextValidator = textValidator;
        }

        /// <summary>
        /// Deserialize val and change the underlying data
        /// </summary>
        /// <param name="val"></param>
        public abstract void Deserialize(string val);

        /// <summary>
        /// Serialize the underlying data into a string
        /// </summary>
        /// <returns></returns>
        public abstract string Serialize();

        public void Load(KeyValueStorage kvs)
        {
            Deserialize(kvs.GetString(StorageKey, DefaultValue));
        }

        public void Save(KeyValueStorage kvs)
        {
            kvs.SetValue(StorageKey, Serialize());
        }

        public bool IsValid
        {
            get
            {
                if (TextValidator == null) return true;
                return TextValidator.Validate(Serialize());
            }
        }

        public string ErrorMessage
        {
            get
            {
                return $"{TextValidator.RequirementMessage}";
            }
        }
    }
}
