using System.Threading.Tasks;

namespace GrandTheftMultiplayer.Launcher.Services.Interfaces
{
	public interface ISelfUpdateService
	{
		event EventDelegates.UpdateStatusChangedHandler UpdateStatusChanged;

		Task<bool> IsUpdateAvailable();

		Task UpdateSelf();
	}
}