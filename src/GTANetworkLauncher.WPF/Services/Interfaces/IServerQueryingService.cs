using System.Threading.Tasks;
using GrandTheftMultiplayer.Shared;

namespace GrandTheftMultiplayer.Launcher.Services.Interfaces
{
    public interface IServerQueryingService
    {
        Task<DiscoveryResponse> QueryServer(Models.ServerApi.Server server);

        Task ClearRefreshQueue();
    }
}