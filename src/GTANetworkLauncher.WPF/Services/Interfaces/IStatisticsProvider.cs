using System.Threading.Tasks;
using GrandTheftMultiplayer.Launcher.Models.Statistics;

namespace GrandTheftMultiplayer.Launcher.Services.Interfaces
{
    public interface IStatisticsProvider
    {
        Task<StatisticsResponse> GetStatistics();
    }
}