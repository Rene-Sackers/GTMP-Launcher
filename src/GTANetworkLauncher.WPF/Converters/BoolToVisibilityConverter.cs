using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GrandTheftMultiplayer.Launcher.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public bool Inverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = value as bool?;
            var checkingValue = boolValue == true;
            checkingValue = Inverse ? !checkingValue : checkingValue;

            return checkingValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
