using System.Diagnostics;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Launcher.Models.GameLauncherService;

namespace GrandTheftMultiplayer.Launcher.Services.Interfaces
{
	public interface IGameLauncherService
	{
		bool IsLaunching { get; }

		Task<Process> LaunchGame(GameLaunchMode launchMode, bool joinServer = false);

	    Task<Process> LaunchGameAndJoinServer(string serverIp, int port);

		void ShutdownGrandTheftMultiplayer();
	}
}