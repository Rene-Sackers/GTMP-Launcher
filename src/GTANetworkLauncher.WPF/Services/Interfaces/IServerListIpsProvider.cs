using System.Collections.Generic;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Launcher.Models.ServerApi;

namespace GrandTheftMultiplayer.Launcher.Services.Interfaces
{
    public interface IServerListIpsProvider
    {
        Task<ICollection<ServerInfo>> GetServerListIps();
    }
}