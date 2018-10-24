using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.WPF.Views.EditableTreeView
{
    public class EditableTreeViewLevelRule
    {
        public string HelpText { get; private set; }
        public bool Unique { get; private set; }

        public EditableTreeViewLevelRule(string helpText, bool unique = false)
        {
            this.HelpText = helpText;
            this.Unique = unique;
        }
    }
}
