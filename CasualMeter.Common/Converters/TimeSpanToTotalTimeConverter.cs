using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CasualMeter.Common.Converters
{
    public class TimeSpanToTotalTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TimeSpan))
                throw new ArgumentException($"Invalid arguments passed to {nameof(TimeSpanToTotalTimeConverter)}.");

            return $"Total time: {((TimeSpan) value).ToString(@"mm\:ss")}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
