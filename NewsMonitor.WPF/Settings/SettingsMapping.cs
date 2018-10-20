using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NewsMonitor.WPF.Settings
{
    public class SettingsMapping
    {
        public string StorageKey { get; private set; }
        public string DefaultValue { get; private set; }
        public TextBox TextBox { get; private set; }
        public TextValidator TextValidator { get; private set; }

        public SettingsMapping(string storageKey, string defaultValue, TextBox textBox,
            Func<string, bool> textValidator)
        {
            this.StorageKey = storageKey;
            this.DefaultValue = defaultValue;
            this.TextBox = textBox;

            this.TextValidator = (textValidator == null ? null :
                new TextValidator(textValidator));
        }

        public SettingsMapping(string storageKey, string defaultValue, TextBox textBox,
            TextValidator textValidator = null)
        {
            this.StorageKey = storageKey;
            this.DefaultValue = defaultValue;
            this.TextBox = textBox;
            this.TextValidator = textValidator;
        }

        public bool IsValid
        {
            get
            {
                if (TextValidator == null) return true;
                return TextValidator.Validate(TextBox.Text);
            }
        }

        public string ErrorMessage
        {
            get
            {
                return $"{TextBox.Name} {TextValidator.RequirementMessage}";
            }
        }
    }
}
