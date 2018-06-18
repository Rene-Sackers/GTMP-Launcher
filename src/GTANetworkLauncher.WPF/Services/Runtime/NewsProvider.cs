using System.Collections.Generic;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Launcher.Helpers;
using GrandTheftMultiplayer.Launcher.Models.Forum;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;

namespace GrandTheftMultiplayer.Launcher.Services.Runtime
{
    public class NewsProvider : INewsProvider
    {
        public async Task<IEnumerable<ForumPostItem>> GetNewsAsync()
        {
            const string urlParams = "?news";
            return await HttpDataHelper.GetDataFromUrl<List<ForumPostItem>>(Constants.ForumsApiUrl + urlParams);
        }
    }
}
