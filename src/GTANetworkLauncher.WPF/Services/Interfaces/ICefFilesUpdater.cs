using System.Threading.Tasks;

namespace GrandTheftMultiplayer.Launcher.Services.Interfaces
{
	public interface ICefFilesUpdater
	{
		Task<bool> UpdateCefBrowserFiles();
	}
}