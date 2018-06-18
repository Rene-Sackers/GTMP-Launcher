using System.Collections.Generic;
using System.Threading.Tasks;
using ServerModel = GrandTheftMultiplayer.Launcher.Models.ServerApi.Server;

namespace GrandTheftMultiplayer.Launcher.Services.Interfaces
{
    public interface IServerListProvider
    {
        event EventDelegates.FavoritedServersUpdatedHandler FavoritedServersUpdated;

        Task<IEnumerable<ServerModel>> GetServers(bool refreshCache = false);
    }
}