using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.WPF.Settings
{
    public class InvalidExtensionTypeException : Exception
    {
        public InvalidExtensionTypeException(string text) : 
            base(text)
        {
            
        }

    }
}
