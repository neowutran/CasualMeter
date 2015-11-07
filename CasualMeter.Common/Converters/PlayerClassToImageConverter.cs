using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using CasualMeter.Common.Helpers;
using Tera.Game;

namespace CasualMeter.Common.Converters
{
    public class PlayerClassToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is PlayerClass) && targetType != typeof(Image))
                throw new ArgumentException();

            return SettingsHelper.Instance.GetImage((PlayerClass) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
