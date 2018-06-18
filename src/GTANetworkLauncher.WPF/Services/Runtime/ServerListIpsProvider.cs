using System.Collections.Generic;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Launcher.Helpers;
using GrandTheftMultiplayer.Launcher.Models.ServerApi;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;

namespace GrandTheftMultiplayer.Launcher.Services.Runtime
{
    public class ServerListIpsProvider : IServerListIpsProvider
    {
        public async Task<ICollection<ServerInfo>> GetServerListIps()
        {
            return (await HttpDataHelper.GetDataFromUrl<ServerApiResponse>(Constants.ServerApiUrl))?.List;
        }
    }
}