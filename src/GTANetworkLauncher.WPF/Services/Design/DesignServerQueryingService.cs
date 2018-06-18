using System;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;
using GrandTheftMultiplayer.Shared;

namespace GrandTheftMultiplayer.Launcher.Services.Design
{
    public class DesignServerQueryingService : IServerQueryingService
    {
        public Task<DiscoveryResponse> QueryServer(Models.ServerApi.Server server)
        {
            return Task.FromResult(new DiscoveryResponse
            {
                Port = server.Port,
                Gamemode = server.GameMode,
                LAN = server.LAN,
                MaxPlayers = server.MaxPlayers,
                PasswordProtected = server.IsPasswordProtected,
                PlayerCount = server.PlayerCount,
                ServerName = server.Name
            });
        }

        public Task ClearRefreshQueue()
        {
            throw new NotImplementedException();
        }
    }
}