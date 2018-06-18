using System;
using System.Globalization;
using System.Windows.Data;

namespace GrandTheftMultiplayer.Launcher.Converters
{
    public class BoolToAnyConverter : IValueConverter
    {
        public object TrueValue { get; set; }
        public object FalseValue { get; set; }
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool?)value == true ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == TrueValue;
        }
    }
}
