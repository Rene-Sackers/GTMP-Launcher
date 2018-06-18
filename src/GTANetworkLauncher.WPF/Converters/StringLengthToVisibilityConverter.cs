using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GrandTheftMultiplayer.Launcher.Converters
{
    public class StringLengthToVisibilityConverter : IValueConverter
    {
        public bool Inverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringValue = value as string;
            var stringHasLength = stringValue?.Length > 0;

            stringHasLength = Inverse ? !stringHasLength : stringHasLength;

            return stringHasLength ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
