using System.Collections.Generic;

namespace GrandTheftMultiplayer.Launcher.Models.Status
{
    public class ServerStatusApiResponse
    {
        public ServerStatusMeta Meta { get; set; }

        public List<ServerStatus> Data { get; set; }
    }
}