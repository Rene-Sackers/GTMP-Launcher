using System;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Launcher.Models.Statistics;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;

namespace GrandTheftMultiplayer.Launcher.Services.Design
{
    public class DesignStatisticsProvider : IStatisticsProvider
    {
        public Task<StatisticsResponse> GetStatistics()
        {
            var random = new Random();
            return Task.FromResult(new StatisticsResponse {TotalPlayersOnline = random.Next(0, 3000), TotalServersOnline = random.Next(0, 200)});
        }
    }
}
