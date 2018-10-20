using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.WPF.Settings
{
    public class TextValidator
    {
        public Func<string, bool> Validate { get; private set; }
        public string RequirementMessage { get; private set; }

        public TextValidator(Func<string, bool> textValidator,
            string requirementMessage = null)
        {
            this.Validate = textValidator;
            this.RequirementMessage = requirementMessage ?? 
                "must be valid";
        }

        public static TextValidator IntegerValidator
        {
            get
            {
                return new TextValidator((s) =>
                {
                    int o;
                    return Int32.TryParse(s, out o);
                }, "must be an integer");
            }
        }
    }
}
