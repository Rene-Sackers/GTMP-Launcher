using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using GrandTheftMultiplayer.Launcher.Helpers;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;

namespace GrandTheftMultiplayer.Launcher.Services.Runtime
{
	public class SelfUpdateService : ISelfUpdateService
	{
	    private const string UpdateFilePath = "tempstorage";
	    private const string UpdateFileName = "launcher_files.zip";

        private readonly ILogger _logger;
		private readonly INotificationService _notificationService;
		private readonly ISettingsProvider _settingsProvider;

		public event EventDelegates.UpdateStatusChangedHandler UpdateStatusChanged;

		public SelfUpdateService(
			ILogger logger,
			INotificationService notificationService,
			ISettingsProvider settingsProvider)
		{
			_logger = logger;
			_notificationService = notificationService;
			_settingsProvider = settingsProvider;
		}

		public async Task<bool> IsUpdateAvailable()
		{
			_logger.Write("Checking for launcher update.");

			var currentVersion = GetCurrentVersion();
			var newestVersion = await TryGetLatestVersion();

			_logger.Write($"Current version: {currentVersion}, newest version: {newestVersion}");

			return newestVersion > currentVersion;
		}

		private async Task<Version> TryGetLatestVersion()
		{
			try
			{
				return await GetLatestLauncherVersion();
			}
			catch (Exception e)
			{
				_logger.Write(e);
				_notificationService.ShowNotification("Failed to check for launcher update.", true);
			}

			return null;
		}

		private async Task<Version> GetLatestLauncherVersion()
		{
			string versionString;
			using (var httpClient = new HttpClient())
				versionString = await httpClient.GetStringAsync(Constants.GetLauncherVersionUrl(_settingsProvider.GetCurrentSettings().UpdateChannel));

			return Version.Parse(versionString);
		}

		private static Version GetCurrentVersion()
		{
		    var executingFileVersion = VersionHelper.VersionFromFile(Assembly.GetExecutingAssembly().Location);

            return executingFileVersion;
		}

		public async Task UpdateSelf()
		{
			var applicationFileName = Path.GetFileName(Assembly.GetExecutingAssembly().Location);
		    FileOperationsHelper.EnsureFileDirectory(GetUpdateDownloadPath());

            if (!await TryDownloadNewLauncher(GetUpdateDownloadPath()))
			{
				_notificationService.ShowNotification("Failed to download new launcher.", true);
				return;
			}

		    if (!await ExtractUpdate(GetUpdateDownloadPath()))
		    {
		        _notificationService.ShowNotification("Failed to extract launcher update.", true);
                return;
		    }

            var updateBatTargetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "update.bat");
			if (!await TryExtractUpdateBat(updateBatTargetPath))
			{
				_notificationService.ShowNotification("Failed to perform update.", true);
				return;
			}

			Process.Start(updateBatTargetPath, applicationFileName);
			Process.GetCurrentProcess().Kill();
		}

	    private async Task<bool> ExtractUpdate(string updateFilePath)
	    {
	        var extractionPath = GetUpdateDownloadFolder();
	        _logger.Write($"Extracting launcher files {updateFilePath} to {extractionPath}");

	        try
	        {
	            await ZipExtractor.ExtractToDirectoryAsync(updateFilePath, extractionPath, true);

                return true;
	        }
	        catch (Exception ex)
	        {
	            _logger.Write(ex);
	        }

	        return false;
	    }

        private async Task<bool> TryExtractUpdateBat(string updateBatTargetPath)
		{
			_logger.Write("Extracting update.bat");

			try
			{
				var resourceStream = Application.GetResourceStream(new Uri("pack://application:,,,/Resources/Update/update.bat"));
			    if (resourceStream == null)
			    {
			        _logger.Write("Could not read update.bat from application resources.");
			        return false;
			    }

				var updateBatBytes = new byte[resourceStream.Stream.Length];
				await resourceStream.Stream.ReadAsync(updateBatBytes, 0, updateBatBytes.Length);

				File.WriteAllBytes(updateBatTargetPath, updateBatBytes);

				return true;
			}
			catch (Exception e)
			{
				_logger.Write(e);
			}

			return false;
		}

		private async Task<bool> TryDownloadNewLauncher(string downloadTargetPath)
		{
			var downloadLauncherUrl = Constants.GetLauncherFileDownloadUrl(_settingsProvider.GetCurrentSettings().UpdateChannel);
			_logger.Write("Downloading new launcher.");

			try
			{
				using (var httpClientWithProgress = new HttpClientDownloadWithProgress(downloadLauncherUrl, downloadTargetPath))
				{
					httpClientWithProgress.ProgressChanged += (size, downloaded, percentage) => UpdateStatusChanged?.Invoke(percentage ?? 50);
					await httpClientWithProgress.StartDownload();
				}

				return true;
			}
			catch (Exception e)
			{
				_logger.Write(e);
			}

			return false;
		}

	    private static string GetUpdateDownloadFolder()
	    {
	        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, UpdateFilePath);
	    }

        private static string GetUpdateDownloadPath()
	    {
	        return Path.Combine(GetUpdateDownloadFolder(), UpdateFileName);
	    }
    }
}