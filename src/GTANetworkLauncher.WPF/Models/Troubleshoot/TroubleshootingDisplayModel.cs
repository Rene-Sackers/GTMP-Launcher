using GrandTheftMultiplayer.Launcher.Models.Forum;

namespace GrandTheftMultiplayer.Launcher.Models.Troubleshoot
{
	public class TroubleshootingDisplayModel
	{
		public ForumPostItem ForumPostItem { get; }
        
		public TroubleshootingDisplayModel(ForumPostItem forumPostItem)
		{
			ForumPostItem = forumPostItem;
			ForumPostItem.Author = "By: " + forumPostItem.Author + ", on ";
		}
	}
}