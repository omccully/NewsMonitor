using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NewsMonitor.WPF.Converters
{
    public class UrlToDomainConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string multiUrl = (string)value;
                string[] urls = multiUrl.Split('\n');
                IEnumerable<string> domains = urls.Select(url => new Uri(url).Host.Replace("www.", ""));
                return String.Join(", ", domains);
            }
            catch
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
