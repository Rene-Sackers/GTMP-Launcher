using System.Collections.Generic;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Launcher.Models.Status;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;

namespace GrandTheftMultiplayer.Launcher.Services.Design
{
    public class DesignServerStatusProvider : IServerStatusProvider
    {
        public Task<ServerStatusApiResponse> GetServerStatusList()
        {
            return Task.FromResult(new ServerStatusApiResponse
            {
                Data = new List<ServerStatus>
                {
                    new ServerStatus
                    {
                        Name = "Operational",
                        Status = ServerStatuses.Operational,
                        StatusName = "Operational",
                        Link = "http://example.com/"
                    },
                    new ServerStatus
                    {
                        Name = "Performance issues",
                        Status = ServerStatuses.PerformanceIssues,
                        StatusName = "Performance issues"
                    },
                    new ServerStatus
                    {
                        Name = "Partial Outage",
                        Status = ServerStatuses.PartialOutage,
                        StatusName = "Partial outage"
                    },
                    new ServerStatus
                    {
                        Name = "Major Outage",
                        Status = ServerStatuses.MajorOutage,
                        StatusName = "Major outage"
                    }
                }
            });
        }
    }
}
