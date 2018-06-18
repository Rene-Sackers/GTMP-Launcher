using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using GrandTheftMultiplayer.Launcher.Models.Status;

namespace GrandTheftMultiplayer.Launcher.Converters
{
    public class ServerStatusToColorConverter : IValueConverter
    {
        public SolidColorBrush OperationalColor { get; set; }
        public SolidColorBrush PerformanceIssuesColor { get; set; }
        public SolidColorBrush PartialOutageColor { get; set; }
        public SolidColorBrush MajorOutageColor { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var serverStatus = value as ServerStatuses?;

            if (!serverStatus.HasValue) return null;

            switch (serverStatus)
            {
                case ServerStatuses.Operational:
                    return OperationalColor;
                case ServerStatuses.PerformanceIssues:
                    return PerformanceIssuesColor;
                case ServerStatuses.PartialOutage:
                    return PartialOutageColor;
                case ServerStatuses.MajorOutage:
                    return MajorOutageColor;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
