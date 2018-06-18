using System.Threading.Tasks;

namespace GrandTheftMultiplayer.Launcher.Services.Interfaces
{
	public interface IGameBackupService
	{
		Task CheckBackupStatus();
	}
}