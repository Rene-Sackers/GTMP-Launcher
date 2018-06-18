using System.IO;
using System.Windows;
using GrandTheftMultiplayer.Launcher.Helpers;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;
using GrandTheftMultiplayer.Shared;

namespace GrandTheftMultiplayer.Launcher.Services.Runtime
{
	public class ServerHostProvider : IServerHostProvider
	{
		private readonly ILogger _logger;
		private readonly ISettingsProvider _settingsProvider;
		private readonly PlayerSettings _settings;

		public ServerHostProvider(
			ILogger logger,
			ISettingsProvider settingsProvider)
		{
			_logger = logger;
			_settingsProvider = settingsProvider;
			_settings = settingsProvider.GetCurrentSettings();
		}

		public string GetGtMpServerDirectoryPath()
		{
			if (IsValidGtMpServerPath(_settings.ServerHostPath))
			{
				_logger.Write($"Valid GT-MP Server path in settings: {_settings.ServerHostPath}");
				return _settings.ServerHostPath;
			}

			while (true)
			{
				var exePath = BrowseForServerExePath();

				_logger.Write($"User manually selected {exePath} as GrandTheftMultiplayer.Server.exe path.");

				if (IsValidGtMpServerPath(exePath))
					return exePath;

				_logger.Write("Selected path is invalid.");

				if (MessageBox.Show("Invalid file selected. Try again?", "Invalid file.", MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes) continue;

				return null;
			}
		}

		private string BrowseForServerExePath()
		{
			var selectedFile = FileOperationsHelper.BrowseForGta5Exe();

			if (!IsValidGtMpServerPath(selectedFile)) return null;

			_settings.ServerHostPath = selectedFile;
			_settingsProvider.SaveSettings(_settings);
			return selectedFile;
		}

		private static bool IsValidGtMpServerPath(string path)
		{
			if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
				return false;

			var exePath = Path.Combine(path, Constants.ServerExeFileName);
			return File.Exists(exePath);
		}
	}
}
