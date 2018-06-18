using System.Threading.Tasks;
using GrandTheftMultiplayer.Launcher.Models.Forum;

namespace GrandTheftMultiplayer.Launcher.Services.Interfaces
{
    public interface ITroubleshootProvider
    {
        Task<ForumPostItem> GetTroubleshootingAsync();
    }
}