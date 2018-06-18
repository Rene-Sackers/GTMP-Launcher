using System.Collections.Generic;

namespace GrandTheftMultiplayer.Launcher.Models.ServerApi
{
    public class ServerApiResponse
    {
        public ICollection<ServerInfo> List { get; set; }
    }
}