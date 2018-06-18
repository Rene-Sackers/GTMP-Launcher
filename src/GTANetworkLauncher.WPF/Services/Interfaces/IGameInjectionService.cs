using System.Diagnostics;

namespace GrandTheftMultiplayer.Launcher.Services.Interfaces
{
	public interface IGameInjectionService
	{
		bool InjectIntoProcess(Process process);

		void InjectCustomAsiFiles(Process targetProcess, string gamePath);
	}
}