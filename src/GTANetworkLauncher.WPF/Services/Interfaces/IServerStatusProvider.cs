using System.Threading.Tasks;
using GrandTheftMultiplayer.Launcher.Models.Status;

namespace GrandTheftMultiplayer.Launcher.Services.Interfaces
{
    public interface IServerStatusProvider
    {
        Task<ServerStatusApiResponse> GetServerStatusList();
    }
}