using System.Threading.Tasks;
using GrandTheftMultiplayer.Launcher.Helpers;
using GrandTheftMultiplayer.Launcher.Models.Statistics;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;

namespace GrandTheftMultiplayer.Launcher.Services.Runtime
{
    public class StatisticsProvider : IStatisticsProvider
    {
        public Task<StatisticsResponse> GetStatistics()
        {
            return HttpDataHelper.GetDataFromUrl<StatisticsResponse>(Constants.StatisticsUrl);
        }
    }
}
