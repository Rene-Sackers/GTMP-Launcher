using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using GrandTheftMultiplayer.Launcher.Helpers;
using GrandTheftMultiplayer.Launcher.Models.Logger;
using GrandTheftMultiplayer.Launcher.Models.Notifications;
using GrandTheftMultiplayer.Launcher.Models.SplashScreenProgressProvider;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;

namespace GrandTheftMultiplayer.Launcher.Services.Runtime
{
	public class ClientFilesUpdater : IClientFilesUpdater
	{
		private const string GtMultiplayerBinFolder = "bin";
		private const string GrandTheftMultiplayerDllRelativePath = GtMultiplayerBinFolder + @"\scripts\GrandTheftMultiplayer.Client.dll";
	    private const string UpdateFolder = @"tempstorage\";
		private const string UpdateFilePath = UpdateFolder + @"files.zip";

		private readonly ILogger _logger;
		private readonly INotificationService _notificationService;
		private readonly ISplashScreenProgressProvider _splashScreenProgressProvider;
		private readonly ISettingsProvider _settingsProvider;

		public ClientFilesUpdater(
			ILogger logger,
			INotificationService notificationService,
			ISplashScreenProgressProvider splashScreenProgressProvider,
			ISettingsProvider settingsProvider)
		{
			_logger = logger;
			_notificationService = notificationService;
			_splashScreenProgressProvider = splashScreenProgressProvider;
			_settingsProvider = settingsProvider;
		}

		public async Task<bool> UpdateClientFiles()
		{
			var currentClientVersion = GetCurrentClientVersion();
			if (currentClientVersion == null)
				return await TryDownloadNewClientFiles();

			var newestClientVersion = await TryGetNewestClientVersion();
			if (newestClientVersion == null)
				return true;

			_logger.Write($"Current client version: {currentClientVersion}, newest client version: {newestClientVersion}");

			if (currentClientVersion == newestClientVersion)
				return true;

#if DEBUG
		    return true;
#endif

            if (!ShouldDownloadNewClientVersion(currentClientVersion, newestClientVersion))
				return true;

			_splashScreenProgressProvider.SetCurrentStep(SplashScreenProgressStep.DownloadNewClient);
			return await TryDownloadNewClientFiles();
		}

		private bool ShouldDownloadNewClientVersion(Version currentVersion, Version newVersion)
		{
			var downloadNewestVersion =
				MessageBox.Show(
					$"New version of GT-MP available.\nCurrent version: {currentVersion}\nNew version: {newVersion}\n\nDownload now?",
					"New Version Available",
					MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes;

			if (!downloadNewestVersion)
				_logger.Write("User chose not to download newest client version.");

			return downloadNewestVersion;
		}

		private async Task<bool> TryDownloadNewClientFiles()
		{
			_logger.Write("Downloading new client files.");
			try
			{
				var success = await DownloadNewClientFiles();
				return success;
			}
			catch (Exception ex)
			{
				_logger.Write(ex);
				_notificationService.ShowNotification("Failed to download client files.", true);
				return false;
			}
		}

		private async Task<bool> DownloadNewClientFiles()
		{
			var updateFilePath = GetUpdateDownloadPath();
			FileOperationsHelper.EnsureFileDirectory(updateFilePath);

			var downloadUpdateUrl = Constants.GetUpdateFileDownloadUrl(_settingsProvider.GetCurrentSettings().UpdateChannel);

			using (var client = new HttpClientDownloadWithProgress(downloadUpdateUrl, updateFilePath))
			{
				//client.ProgressChanged += (size, downloaded, percentage) =>
				//	_splashScreenProgressProvider.UpdateStep(LaunchStep.DownloadNewClient, LaunchStepStatus.Processing, percentage);

				await client.StartDownload();
			}

			if (File.Exists(updateFilePath)) return await ExtractUpdate(updateFilePath);

			_notificationService.ShowNotification("Failed to download update.", true);
			return false;
		}

		private async Task<bool> ExtractUpdate(string updateFilePath)
		{
			var extractionPath = GetUpdateExtractionPath();
			_logger.Write($"Extracting client files {updateFilePath} to {extractionPath}");

			try
			{
			    CleanupClientDirectory(extractionPath);
                await ZipExtractor.ExtractToDirectoryAsync(updateFilePath, extractionPath, true);
			    Directory.Delete(GetUpdateDownloadFolder(), true);

                return true;
			}
			catch (Exception ex)
			{
				_logger.Write(ex);
				_notificationService.ShowNotification("Failed to extract update.", true);
			}

			return false;
		}

		private Version GetCurrentClientVersion()
		{
			var grandTheftMultiplayerDllPath = GetGrandTheftMultiplayerkDllPath();
			_logger.Write($"Checking current client version of file {grandTheftMultiplayerDllPath}");

			if (!File.Exists(grandTheftMultiplayerDllPath))
			{
				_logger.Write("File is missing.", LogMessageSeverity.Error);
				_notificationService.ShowNotification($"Missing \"{grandTheftMultiplayerDllPath}\".", Notification.ShortMessageDelay);
				return null;
			}
			
			var version = VersionHelper.VersionFromFile(grandTheftMultiplayerDllPath);
			_logger.Write($"Current client version: {version}");
			return version;
		}

		private async Task<Version> TryGetNewestClientVersion()
		{
			try
			{
				_logger.Write("Getting newest client files version.");
				return await GetNewestClientVersion();
			}
			catch (Exception ex)
			{
				_logger.Write(ex);
				_notificationService.ShowNotification("Failed to check for newest client version.", true);
			}

			return null;
		}
		
		private async Task<Version> GetNewestClientVersion()
		{
			string versionString;
			using (var httpClient = new HttpClient())
				versionString = await httpClient.GetStringAsync(Constants.GetClientVersionUrl(_settingsProvider.GetCurrentSettings().UpdateChannel));

			return Version.Parse(versionString);
		}

	    private void CleanupClientDirectory(string path)
	    {
	        foreach (var file in Directory.GetFiles(path, "*.pdb", SearchOption.AllDirectories))
	        {
	            File.Delete(file);
	        }
        }

		private static string GetGrandTheftMultiplayerkDllPath()
		{
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GrandTheftMultiplayerDllRelativePath);
		}

	    private static string GetUpdateDownloadFolder()
	    {
	        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, UpdateFolder);
	    }

        private static string GetUpdateDownloadPath()
		{
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, UpdateFilePath);
		}

		private static string GetUpdateExtractionPath()
		{
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GtMultiplayerBinFolder);
		}
	}
}