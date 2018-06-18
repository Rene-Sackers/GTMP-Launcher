using System.Threading.Tasks;
using GrandTheftMultiplayer.Launcher.Helpers;
using GrandTheftMultiplayer.Launcher.Models.Forum;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;

namespace GrandTheftMultiplayer.Launcher.Services.Runtime
{
    public class TroubleshootProvider : ITroubleshootProvider
    {
        private const int TroubleshootingPostId = 4657;

        public async Task<ForumPostItem> GetTroubleshootingAsync()
        {
            var urlParams = $"?action=getPost&hash={Constants.ForumsApiKey}&value=" + TroubleshootingPostId;
            var forumPost = await HttpDataHelper.GetDataFromUrl<ForumPostItem>(Constants.ForumsApiUrl + urlParams);

            return await Task.FromResult(forumPost);
        }
    }
}
