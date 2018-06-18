using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GrandTheftMultiplayer.Launcher.Helpers;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;

namespace GrandTheftMultiplayer.Launcher.Services.Runtime
{
	public class GameBackupService : IGameBackupService
	{
	    private const string BackupFolder = "GameBackups";

	    private readonly ILogger _logger;
	    private readonly ISettingsProvider _settingsProvider;
	    private readonly INotificationService _notificationService;
		private readonly IGtaVersionProvider _gtaVersionProvider;
		private readonly string _gamePath;

		public GameBackupService(
            ILogger logger,
			ISettingsProvider settingsProvider,
			INotificationService notificationService,
			IGtaVersionProvider gtaVersionProvider)
		{
		    _logger = logger;
		    _settingsProvider = settingsProvider;
		    _notificationService = notificationService;
			_gtaVersionProvider = gtaVersionProvider;

			_gamePath = _settingsProvider.GetCurrentSettings().GamePath;
		}

		public async Task CheckBackupStatus()
		{
			if (string.IsNullOrWhiteSpace(_gamePath) || !Directory.Exists(_gamePath))
				return;

		    var currentGta5Version = _gtaVersionProvider.GetCurrentGtaVersion();
		    if (currentGta5Version == null)
		    {
		        _notificationService.ShowNotification("Failed to check current GTA 5 version.", true);
		        return;
		    }

            if (!GtaBackupVersionExists(currentGta5Version.Build.ToString(), GetGtaGameType(_gamePath)))
			{
				AskToBackUpGame();
				return;
			}

			var supportedGta5Version = await _gtaVersionProvider.GetCurrentlySupportedGtaVersion();
			if (supportedGta5Version == null)
			{
				_notificationService.ShowNotification("Failed to check currently supported GTA 5 version.", true);
				return;
			}

			int lastGtaBackupVersion = GetBackupVersionsByType(GetGtaGameType(_gamePath)).Min();
		    if (lastGtaBackupVersion > supportedGta5Version.Build)
		    {
		        _notificationService.ShowNotification("GTA 5 can not be downgraded. Last backup game version is not supported.", true);
                _logger.Write("GTA 5 can not be downgraded. Last backup game version is not supported.");
		        return;
		    }

            if (currentGta5Version > supportedGta5Version && AskToRestoreBackup(supportedGta5Version, currentGta5Version, lastGtaBackupVersion))
				await RestoreBackup(lastGtaBackupVersion, GetGtaGameType(_gamePath));
		}

		private static bool AskToRestoreBackup(Version supportedGta5Version, Version currentGta5Version, int backupMinorVersion)
		{
			return
				MessageBox.Show(
					$"Grand Theft Multiplayer has detected that the installed GTA 5 version is: {currentGta5Version}, but the supported version is: {supportedGta5Version}\n" +
					$"You have a backed up version: {backupMinorVersion}\n" +
					"Would you like to restore it?",
					"Supported version mismatch",
					MessageBoxButton.YesNoCancel
					) == MessageBoxResult.Yes;
		}

		private void AskToBackUpGame()
		{
		    var queryResult =
				MessageBox.Show(
					"Grand Theft Multiplayer has detected that there is currently no backup of the game files.\n" +
					"Rockstar may update the game at any time, potentially breaking GT-MP.\n" +
					"Would you like to back up critical game files, so that you may restore them if the game updates?\n\n" +
                    "No = no, and don't ask again (can be turned on again in the settings)\n" +
                    "Cancel = not now, but ask again next time",
					"Create backup",
					MessageBoxButton.YesNoCancel);

		    if (queryResult == MessageBoxResult.Yes)
		    {
		        CreateBackup();
		    }
		    else if (queryResult == MessageBoxResult.No)
		    {
		        var currentSettings = _settingsProvider.GetCurrentSettings();
		        currentSettings.AskToBackUpGame = false;
		        _settingsProvider.SaveSettings(currentSettings);
		    }
		}
        
		private static string GetBackupPathByVersion(string gtaMinorVersion, string gameType)
		{
			return Path.Combine(GetBackupPath(), $"downgrade_{gtaMinorVersion}_{gameType}.zip");
		}

	    private static bool GtaBackupVersionExists(string gtaMinorVersion, string gameType)
	    {
	        return Directory.Exists(GetBackupPath()) && File.Exists(GetBackupPathByVersion(gtaMinorVersion, gameType));
	    }

        private static string GetBackupPath()
		{
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, BackupFolder);
		}

	    private static IEnumerable<int> GetBackupVersionsByType(string gameType)
	    {
	        if (!Directory.Exists(GetBackupPath())) yield break;

	        foreach (var backupFile in Directory.GetFiles(GetBackupPath()))
	        {
	            var backupDetails = backupFile.Split('_');
	            if (backupDetails[2] == gameType + ".zip")
	            {
	                yield return Convert.ToInt32(backupDetails[1]);
	            }
	        }
	    }

        private static string GetGtaGameType(string gamePath)
	    {
            return File.Exists(Path.Combine(gamePath, "steam_api64.dll")) ? "steam" : "rgsc";
	    }

        private string[] GetRelativeBackupFilePaths()
		{
			const string updateRpfPath = "update/update.rpf";
			const string gta5ExecutablePath = Constants.GTA5ExecutableFileName;
			const string gta5LauncherExecutablePath = "GTAVLauncher.exe";
			
			var relativeFilePaths = new List<string>
			{
				updateRpfPath,
				gta5ExecutablePath,
				gta5LauncherExecutablePath
			};

			const string socialClubLauncherExecutablePath = "PlayGTA5.exe";
			const string steamApiDllPath = "steam_api64.dll";
			
			if (File.Exists(Path.Combine(_gamePath, socialClubLauncherExecutablePath)))
				relativeFilePaths.Add(socialClubLauncherExecutablePath);

			if (File.Exists(Path.Combine(_gamePath, steamApiDllPath)))
				relativeFilePaths.Add(steamApiDllPath);

			return relativeFilePaths.ToArray();
		}

		private void CreateBackup()
		{
			var relativeFilePaths = GetRelativeBackupFilePaths();
			var absoluteSourcePaths = relativeFilePaths.Select(fp => Path.Combine(_gamePath, fp)).ToList();

		    var backupVersion = _gtaVersionProvider.GetCurrentGtaVersion().Build;
            var backupName = $"downgrade_{backupVersion}_{GetGtaGameType(_gamePath)}";
		    var backupPath = Path.Combine(GetBackupPath(), backupName);
		    var archivePath = Path.Combine(GetBackupPath(), backupName + ".zip");
            var totalBackupSize = FileOperationsHelper.CalculateTotalFileSizes(absoluteSourcePaths);
			var readableBackupSize = FileOperationsHelper.BytesToReadableString(totalBackupSize);

			if (MessageBox.Show($"The backup will take {readableBackupSize} of disk space and might take some minutes. Continue?", "Backup disk usage", MessageBoxButton.YesNoCancel) != MessageBoxResult.Yes)
				return;

            _logger.Write($"Backup has started. ( Type = {GetGtaGameType(_gamePath)}, Gta 5 Version = {_gtaVersionProvider.GetCurrentGtaVersion()} )");
            FileOperationsHelper.CopyFiles(_gamePath, backupPath, relativeFilePaths);
            _logger.Write("Backup has completed.");
            _logger.Write("Archiving Backup.");
            ZipFile.CreateFromDirectory(backupPath, archivePath, CompressionLevel.NoCompression, false);
            Directory.Delete(backupPath, true);
            _logger.Write("Archiving completed.");
		    _notificationService.ShowNotification("Successfully created a backup of your GTA 5 files.");
        }

        private async Task RestoreBackup(int backupVersion, string gameType)
		{
            if (MessageBox.Show("Restoring the backup will overwrite some of your GTA 5 files. Continue?", "Restore backup", MessageBoxButton.YesNoCancel) != MessageBoxResult.Yes)
                return;

            _logger.Write($"Started restoring game backup of version {backupVersion}");

		    var backupZipPath = GetBackupPathByVersion(backupVersion.ToString(), gameType);
		    var backupExtractionPath = _gamePath;

            await ZipExtractor.ExtractToDirectoryAsync(backupZipPath, backupExtractionPath, true);

            _notificationService.ShowNotification("GTA 5 successfully downgraded.");
        }
    }
}