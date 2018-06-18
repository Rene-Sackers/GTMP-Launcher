using System;
using System.Threading.Tasks;

namespace GrandTheftMultiplayer.Launcher.Services.Interfaces
{
	public interface IGtaVersionProvider
	{
		Version GetCurrentGtaVersion();

		Task<Version> GetCurrentlySupportedGtaVersion();
	}
}