using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;
using ServerModel = GrandTheftMultiplayer.Launcher.Models.ServerApi.Server;

namespace GrandTheftMultiplayer.Launcher.Services.Design
{
    public class DesignServerListProvider : IServerListProvider
    {
        private readonly IServerQueryingService _serverQueryingService;

        public event EventDelegates.FavoritedServersUpdatedHandler FavoritedServersUpdated;

        public DesignServerListProvider(IServerQueryingService serverQueryingService)
        {
            _serverQueryingService = serverQueryingService;
        }

        public Task<IEnumerable<ServerModel>> GetServers(bool refreshCache = false)
        {
            return Task.FromResult(CreateExampleServers());
        }

        private IEnumerable<ServerModel> CreateExampleServers()
        {
            var random = new Random();

            for (var i = 0; i < 50; i++)
            {
                var maxPlayers = random.Next(1, 1001);

                yield return new ServerModel(random.Next(0, 99999999).ToString(), _serverQueryingService) {
                    IsFavorited = random.Next(0, 2) == 1,
                    IsVerified = random.Next(0, 2) == 1,
                    IsRecent = random.Next(0, 2) == 1,
                    Name = "Server " + random.Next(0, 101),
                    GameMode = "gamemode-" + random.Next(0, 11),
                    Map = "None",
                    MaxPlayers = (short)maxPlayers,
                    PlayerCount = (short)random.Next(0, maxPlayers),
                    Ping = (short)random.Next(0, maxPlayers),
                    LAN = false,
                    IsPasswordProtected = random.Next(0, 2) == 1,
                    IsQueried = true
                };
            }
        }
    }
}
