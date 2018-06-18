using System;
using System.Globalization;
using System.Windows.Data;

namespace GrandTheftMultiplayer.Launcher.Converters
{
    public class ServerInfoToPlayerCountConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2) return "-";

            if (!int.TryParse(values[0]?.ToString(), out var currentPlayers)) return "-";
            if (!int.TryParse(values[1]?.ToString(), out var maxPlayers) || maxPlayers == 0) return "-";
            
            return currentPlayers + "/" + maxPlayers;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
