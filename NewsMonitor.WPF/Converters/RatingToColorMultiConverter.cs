using NewsMonitor.Data.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace NewsMonitor.WPF.Converters
{
    class RatingToColorMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int articleRating = (int)values[0];
            bool userSetRating = (bool)values[1];
            string ratingStr = values[2].ToString();
            int buttonRating = Int32.Parse(ratingStr);

            if(articleRating == buttonRating)
            {
                if(userSetRating)
                {
                    return new SolidColorBrush(Colors.Green);
                }
                else
                {
                    return new SolidColorBrush(Colors.Orange);
                }
            }
            else
            {
                return new SolidColorBrush(Colors.LightGray);
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
