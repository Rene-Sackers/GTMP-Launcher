using Newtonsoft.Json.Converters;

namespace GrandTheftMultiplayer.Launcher.Helpers
{
    public class MicrosoftDateTimeConverter : IsoDateTimeConverter
    {
        public MicrosoftDateTimeConverter()
        {
            DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        }
    }
}
