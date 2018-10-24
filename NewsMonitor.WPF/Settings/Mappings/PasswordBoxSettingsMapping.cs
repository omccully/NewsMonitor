using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NewsMonitor.WPF.Settings
{
    public class PasswordBoxSettingsMapping : SettingsMapping
    {
        PasswordBox PwBox;

        public PasswordBoxSettingsMapping(string storageKey, string defaultValue,
            PasswordBox pwBox, TextValidator textValidator = null)
            : base(storageKey, defaultValue, textValidator)
        {
            this.PwBox = pwBox;
        }

        public override void Deserialize(string val)
        {
            PwBox.Password = val;
        }

        public override string Serialize()
        {
            return PwBox.Password;
        }
    }
}
