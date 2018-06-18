using System.Threading.Tasks;
using GrandTheftMultiplayer.Launcher.Helpers;
using GrandTheftMultiplayer.Launcher.Models.Status;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;

namespace GrandTheftMultiplayer.Launcher.Services.Runtime
{
    public class ServerStatusProvider : IServerStatusProvider
    {
        public Task<ServerStatusApiResponse> GetServerStatusList()
        {
            return HttpDataHelper.GetDataFromUrl<ServerStatusApiResponse>(Constants.ServerStatusUrl);
        }
    }
}
