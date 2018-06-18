using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Launcher.Helpers;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;

namespace GrandTheftMultiplayer.Launcher.Services.Runtime
{
	public class GtaVersionProvider : IGtaVersionProvider
	{
		private readonly ILogger _logger;
	    private readonly ISettingsProvider _settingsProvider;

		public GtaVersionProvider(ILogger logger, ISettingsProvider settingsProvider)
		{
			_logger = logger;
		    _settingsProvider = settingsProvider;
		}

		public Version GetCurrentGtaVersion()
		{
			var gta5ExecutablePath = GetGta5ExecutablePath();
			return VersionHelper.VersionFromFile(gta5ExecutablePath);
		}

		public async Task<Version> GetCurrentlySupportedGtaVersion()
		{
			_logger.Write("Trying to get currently supported GTA version.");

			try
			{
				return await GetSupportedGtaVersionFromApi();
			}
			catch (Exception ex)
			{
				_logger.Write(ex);
				return null;
			}
		}

		private async Task<Version> GetSupportedGtaVersionFromApi()
		{
			string versionString;
			using (var httpClient = new HttpClient())
				versionString = await httpClient.GetStringAsync(Constants.GetSupportedGta5VersionUrl(_settingsProvider.GetCurrentSettings().UpdateChannel));

			return Version.Parse(versionString);
		}

		private string GetGta5ExecutablePath()
		{
		    var gamePath = _settingsProvider.GetCurrentSettings().GamePath;
		    return string.IsNullOrEmpty(gamePath) ? null : Path.Combine(gamePath, Constants.GTA5ExecutableFileName);
		}
	}
}