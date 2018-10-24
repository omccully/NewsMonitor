using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NewsMonitor.WPF.Settings
{
    public class TextBoxSettingsMapping : SettingsMapping
    {
        TextBox TextBox;

        public TextBoxSettingsMapping(string storageKey, string defaultValue, 
            TextBox textBox, TextValidator textValidator = null)
            : base(storageKey, defaultValue, textValidator)
        {
            this.TextBox = textBox;
        }

        public override void Deserialize(string val)
        {
            TextBox.Text = val;
        }

        public override string Serialize()
        {
            return TextBox.Text;
        }
    }
}
