using System.Collections.Generic;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Launcher.Models.ServerApi;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;

namespace GrandTheftMultiplayer.Launcher.Services.Testing
{
    public class LocalServerListIpsService : IServerListIpsProvider
    {
        public Task<ICollection<ServerInfo>> GetServerListIps()
        {
            return Task.FromResult<ICollection<ServerInfo>>(new []
            {
                new ServerInfo { IpPortAddress = "127.0.0.1:4499"}
            });
        }
    }
}