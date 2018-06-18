using System.Collections.Generic;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Launcher.Models.Forum;

namespace GrandTheftMultiplayer.Launcher.Services.Interfaces
{
    public interface INewsProvider
    {
        Task<IEnumerable<ForumPostItem>> GetNewsAsync();
    }
}