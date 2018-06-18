using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using GrandTheftMultiplayer.Launcher.Models.Logger;
using GrandTheftMultiplayer.Launcher.Models.Notifications;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;

namespace GrandTheftMultiplayer.Launcher.Services.Runtime
{
	public class CefFilesUpdater : ICefFilesUpdater
	{
		private const string LibCefRelativePath = @"cef\libcef.dll";

		private readonly ILogger _logger;
		private readonly INotificationService _notificationService;
		private readonly ISettingsProvider _settingsProvider;

		public CefFilesUpdater(
			ILogger logger,
			INotificationService notificationService,
			ISettingsProvider settingsProvider)
		{
			_logger = logger;
			_notificationService = notificationService;
			_settingsProvider = settingsProvider;
		}

		public async Task<bool> UpdateCefBrowserFiles()
		{
			_logger.Write("Checking for CEF update.");

			var libCefPath = GetLibCefPath();
			var libCefFileExists = File.Exists(libCefPath);

			_logger.Write($"CEF lib path: {libCefPath}");

			if (!libCefFileExists)
			{
				_logger.Write("CEF lib missing.", LogMessageSeverity.Error);
				_notificationService.ShowNotification($"Could not find file \"{libCefPath}\". Please re-install GT-MP.", true);
				return false;
			}

			var currentCefVersion = FileVersionInfo.GetVersionInfo(libCefPath);

			string newestCefVersion;
			try
			{
				newestCefVersion = await GetNewestCefVersion();
			}
			catch (Exception ex)
			{
				_logger.Write(ex);
				_notificationService.ShowNotification("Failed to check current CEF version.", true);
				return true;
			}

			_logger.Write($"Current CEF version: {currentCefVersion.ProductVersion}, newest CEF version: {newestCefVersion}");

			if (currentCefVersion.ProductVersion == newestCefVersion)
			{
				_logger.Write("Same version.");
				return true;
			}

			if (!PromptUserForCefUpdate())
			{
				_logger.Write("User chose not to update.");
				return true;
			}

			_notificationService.ShowNotification("CEF update available.", Notification.ShortMessageDelay);

			Process.Start("https://.../");
			Process.GetCurrentProcess().Kill();

			return false;
		}

		private static bool PromptUserForCefUpdate()
		{
			return
				MessageBox.Show(
					"A new version of CEF is available!\nTo update to it, you'll have to re-install Grand Theft Multiplayer.\nDo this now?",
					"CEF Update Available", MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes;
		}
		
		private static string GetLibCefPath()
		{
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LibCefRelativePath);
		}

		private async Task<string> GetNewestCefVersion()
		{
			string versionString;
			using (var httpClient = new HttpClient())
				versionString = await httpClient.GetStringAsync(Constants.GetCEFVersionUrl(_settingsProvider.GetCurrentSettings().UpdateChannel));

			return versionString;
		}
	}
}