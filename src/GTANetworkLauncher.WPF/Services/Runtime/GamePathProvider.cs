using System;
using System.IO;
using System.Windows;
using GrandTheftMultiplayer.Launcher.Helpers;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;
using GrandTheftMultiplayer.Shared;
using Microsoft.Win32;

namespace GrandTheftMultiplayer.Launcher.Services.Runtime
{
	public class GamePathProvider : IGamePathProvider
	{
		private readonly ILogger _logger;
		private readonly ISettingsProvider _settingsProvider;
	    private readonly PlayerSettings _settings;

		public GamePathProvider(
			ILogger logger,
			ISettingsProvider settingsProvider)
		{
			_logger = logger;
			_settingsProvider = settingsProvider;
		    _settings = settingsProvider.GetCurrentSettings();
		}

		public string GetGta5DirectoryPath()
		{
			if (IsValidGtaPath(_settings.GamePath))
			{
				_logger.Write($"Valid GTA 5 path in settings: {_settings.GamePath}");
				return _settings.GamePath;
			}

			_settings.GamePath = TryFindGtaPath();

			if (IsValidGtaPath(_settings.GamePath))
			{
				_logger.Write($"Found valid GTA path from registry: {_settings.GamePath}");

				_settingsProvider.SaveSettings(_settings);
				return _settings.GamePath;
			}

			MessageBox.Show($"Failed to find {Constants.GTA5ExecutableFileName}. Please select it after pressing OK.");

			while (true)
			{
				var exePath = BrowseForGta5ExePath();

				if (IsValidGtaPath(exePath))
					return exePath;

				_logger.Write("Selected path is invalid.");

				if (MessageBox.Show("Invalid file selected. Try again?", "Invalid file.", MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes) continue;
				
				return null;
			}
		}

		private string BrowseForGta5ExePath()
		{
			var selectedFile = FileOperationsHelper.BrowseForGta5Exe();

		    _logger.Write($"User manually selected {selectedFile} as GTA5.exe path.");

            var selectedFolder = string.IsNullOrEmpty(selectedFile) ? null : Path.GetDirectoryName(selectedFile);

			if (!IsValidGtaPath(selectedFolder)) return null;

			_settings.GamePath = selectedFolder;
			_settingsProvider.SaveSettings(_settings);
			return selectedFile;
		}

		private static bool IsValidGtaPath(string path)
		{
			if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
				return false;

			var exePath = Path.Combine(path, Constants.GTA5ExecutableFileName);
			return File.Exists(exePath);
		}

		private string TryFindGtaPath()
		{
			_logger.Write("Trying to get GTA path from registry.");

			try
			{
				return FindGtaPath();
			}
			catch (Exception e)
			{
				_logger.Write(e);
				return null;
			}
		}

		private static string FindGtaPath()
		{
			// Steam
			const string steamInstallRegistryPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Rockstar Games\GTAV";
			const string steamInstallValueName = "InstallFolderSteam";

			var steamInstallPath = Registry.GetValue(steamInstallRegistryPath, steamInstallValueName, null) as string ?? "";
			var fullInstallPath = steamInstallPath.Replace(@"\GTAV", "");

			if (IsValidGtaPath(fullInstallPath))
				return fullInstallPath;

			// R* Warehouse
			const string rockstarInstallRegistryPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Rockstar Games\Grand Theft Auto V";
			const string rockstarInstallValueName = "InstallFolder";

			var rockstarInstallPath = Registry.GetValue(rockstarInstallRegistryPath, rockstarInstallValueName, null) as string ?? "";
			fullInstallPath = rockstarInstallPath;

			return IsValidGtaPath(fullInstallPath) ? fullInstallPath : null;
		}
	}
}