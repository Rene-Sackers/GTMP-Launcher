using System.Threading.Tasks;

namespace GrandTheftMultiplayer.Launcher.Services.Interfaces
{
	public interface IClientFilesUpdater
	{
		Task<bool> UpdateClientFiles();
	}
}