using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using GrandTheftMultiplayer.Launcher.Helpers;
using GrandTheftMultiplayer.Launcher.Models.GameLauncherService;
using GrandTheftMultiplayer.Launcher.Models.Logger;
using GrandTheftMultiplayer.Launcher.Models.Notifications;
using GrandTheftMultiplayer.Launcher.Models.SplashScreenProgressProvider;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;
using MessageBox = System.Windows.MessageBox;

namespace GrandTheftMultiplayer.Launcher.Services.Runtime
{
	public class GameLauncherService : IGameLauncherService
	{
		private const int ProcessKillTimeout = 5000;
		private const int WaitForStartTimeout = 5 * 60 * 1000; // 5 minutes. If this is not enough, you should throw out our IDE HDD and Pentium 3 CPU :\
		private const string DisabledModsFolderName = "Disabled";
		private const string PcSettingsFileName = "pc_settings.bin";
		private const string Gta5ProcessName = "GTA5";

		public bool IsLaunching { get; private set; }

		private readonly string[] _blockingProcesses =
		{
			Gta5ProcessName,
			"GTAVLauncher"
		};
		private readonly string[] _modFiles =
		{
			"ClearScript.dll",
			"ClearScriptV8-32.dll",
			"ClearScriptV8-64.dll",
			"EasyHook64.dll",
			"scripthookv.dll",
			"ScriptHookVDotNet.dll",
			"dinput8.dll",
			"v8-ia32.dll"
		};

		private readonly ILogger _logger;
		private readonly INotificationService _notificationService;
		private readonly ISplashScreenProgressProvider _splashScreenProgressProvider;
		private readonly ILaunchDependencyCheckService _launchDependencyCheckService;
		private readonly ICefFilesUpdater _cefFilesUpdater;
		private readonly IClientFilesUpdater _clientFilesUpdater;
		private readonly IGamePathProvider _gamePathProvider;
		private readonly IGtaVersionProvider _gtaVersionProvider;
		private readonly IGameInjectionService _gameInjectionService;
	    private readonly ISocialClubCommandlineService _socialClubCommandlineService;
	    private readonly ILowKeySuppressionService _lowKeySuppressionService;
	    private readonly ISettingsProvider _settingsProvider;
	    private readonly IEnsureRegistryKeyService _ensureRegistryKeyService;

	    public GameLauncherService(
			ILogger logger,
			INotificationService notificationService,
			ISplashScreenProgressProvider splashScreenProgressProvider,
			ILaunchDependencyCheckService launchDependencyCheckService,
			ICefFilesUpdater cefFilesUpdater,
			IClientFilesUpdater clientFilesUpdater,
			IGamePathProvider gamePathProvider,
			IGtaVersionProvider gtaVersionProvider,
			IGameInjectionService gameInjectionService,
            ISocialClubCommandlineService socialClubCommandlineService,
            ILowKeySuppressionService lowKeySuppressionService,
            ISettingsProvider settingsProvider,
            IEnsureRegistryKeyService ensureRegistryKeyService)
		{
			_logger = logger;
			_notificationService = notificationService;
			_splashScreenProgressProvider = splashScreenProgressProvider;
			_launchDependencyCheckService = launchDependencyCheckService;
			_cefFilesUpdater = cefFilesUpdater;
			_clientFilesUpdater = clientFilesUpdater;
			_gamePathProvider = gamePathProvider;
			_gtaVersionProvider = gtaVersionProvider;
			_gameInjectionService = gameInjectionService;
		    _socialClubCommandlineService = socialClubCommandlineService;
		    _lowKeySuppressionService = lowKeySuppressionService;
		    _settingsProvider = settingsProvider;
		    _ensureRegistryKeyService = ensureRegistryKeyService;
		}

		public async Task<Process> LaunchGame(GameLaunchMode launchMode, bool joinServer = false)
		{
            if (!_settingsProvider.GetCurrentSettings().AcceptedEula) return null;

            if (IsLaunching) return null;

			IsLaunching = true;

		    if (!joinServer)
		        SetJoinServerAddress(null);

            _logger.Write($"Launching game. ({launchMode})");

			_notificationService.ShowNotification("Launching game...", Notification.ShortMessageDelay);
			_splashScreenProgressProvider.SetCurrentStep(SplashScreenProgressStep.Initializing);

			var gameProcess = launchMode == GameLaunchMode.Multiplayer ? await ExecuteGrandTheftMultiplayerLaunchSteps() : await ExecuteGtaLaunchSteps();

			IsLaunching = false;

			return gameProcess;
		}

	    public Task<Process> LaunchGameAndJoinServer(string serverIp, int port)
	    {
            SetJoinServerAddress($"{serverIp}:{port}");

	        return LaunchGame(GameLaunchMode.Multiplayer, true);
	    }

	    private void SetJoinServerAddress(string joinServerAddress)
	    {
	        var currentSettings = _settingsProvider.GetCurrentSettings();
	        if (currentSettings.JoinServerIp == joinServerAddress)
	            return;

	        currentSettings.JoinServerIp = joinServerAddress;
	        _settingsProvider.SaveSettings(currentSettings);
        }

        private async Task<Process> ExecuteGtaLaunchSteps()
		{
			_splashScreenProgressProvider.SetCurrentStep(SplashScreenProgressStep.CheckingRunningProcesses);
			if (!await CheckRunningProcesses()) return null;
			
			_splashScreenProgressProvider.SetCurrentStep(SplashScreenProgressStep.VerifyingGta5Path);
			var gamePath = _gamePathProvider.GetGta5DirectoryPath();
			if (string.IsNullOrWhiteSpace(gamePath)) return null;

            _splashScreenProgressProvider.SetCurrentStep(SplashScreenProgressStep.CreatingCommandLineArgumentsFile);
		    if (!_socialClubCommandlineService.TryEnsureOfflineModeCommandLine(false)) return null;

            _splashScreenProgressProvider.SetCurrentStep(SplashScreenProgressStep.LaunchingSingleplayerGame);
			LaunchGame(gamePath);

			_splashScreenProgressProvider.SetCurrentStep(SplashScreenProgressStep.WaitingForGameToStart);
			var gameProcess = await WaitForGameStart();
			if (gameProcess == null)
			{
				return null;
			}
			
			_splashScreenProgressProvider.SetCurrentStep(SplashScreenProgressStep.Done);

			DuplicateProcessKiller(gameProcess);

			return gameProcess;
		}

		private async Task<Process> ExecuteGrandTheftMultiplayerLaunchSteps()
		{
			_splashScreenProgressProvider.SetCurrentStep(SplashScreenProgressStep.CheckingRunningProcesses);
			if (!await CheckRunningProcesses()) return null;

			_splashScreenProgressProvider.SetCurrentStep(SplashScreenProgressStep.CheckingDependencies);
			if (!CheckLaunchDependancies()) return null;

			_splashScreenProgressProvider.SetCurrentStep(SplashScreenProgressStep.CheckingCefVersion);
			if (!await _cefFilesUpdater.UpdateCefBrowserFiles()) return null;

			_splashScreenProgressProvider.SetCurrentStep(SplashScreenProgressStep.CheckingClientVersion);
			if (!await _clientFilesUpdater.UpdateClientFiles()) return null;

			_splashScreenProgressProvider.SetCurrentStep(SplashScreenProgressStep.VerifyingGta5Path);
			var gamePath = _gamePathProvider.GetGta5DirectoryPath();
			if (string.IsNullOrWhiteSpace(gamePath)) return null;

		    _splashScreenProgressProvider.SetCurrentStep(SplashScreenProgressStep.EnsuringGamePathRegistryKey);
		    _ensureRegistryKeyService.TryEnsureRegistryKey();

            _splashScreenProgressProvider.SetCurrentStep(SplashScreenProgressStep.CheckingGta5Version);
			if (!await CheckGta5Version()) return null;

			_splashScreenProgressProvider.SetCurrentStep(SplashScreenProgressStep.ApplyingCustomSaveGame);
			if (!ApplyFixedSaveGame()) return null;

			_splashScreenProgressProvider.SetCurrentStep(SplashScreenProgressStep.DisablingActiveMods);
			MoveActiveModsToDisabledFolder(gamePath);

			_splashScreenProgressProvider.SetCurrentStep(SplashScreenProgressStep.ApplyingGameSettings);
			if (!ApplyGameSettings()) return null;

			_splashScreenProgressProvider.SetCurrentStep(SplashScreenProgressStep.PatchingGameStartup);
			if (!PatchStartup()) return null;

			_splashScreenProgressProvider.SetCurrentStep(SplashScreenProgressStep.CreatingCommandLineArgumentsFile);
			if (!_socialClubCommandlineService.TryEnsureOfflineModeCommandLine(true)) return null;

			_splashScreenProgressProvider.SetCurrentStep(SplashScreenProgressStep.LaunchingMultiplayerGame);
			LaunchGame(gamePath);

			_splashScreenProgressProvider.SetCurrentStep(SplashScreenProgressStep.WaitingForGameToStart);
			var gameProcess = await WaitForGameStart();
			if (gameProcess == null)
			{
				return null;
			}

			_splashScreenProgressProvider.SetCurrentStep(SplashScreenProgressStep.InjectingGrandTheftMultiplayer);
			if (!SetUpMod(gameProcess, gamePath)) return null;
			
			_splashScreenProgressProvider.SetCurrentStep(SplashScreenProgressStep.Done);

			DuplicateProcessKiller(gameProcess);

//#if !DEBUG
//            SuppressKeys(gameProcess);
//#endif

            return gameProcess;
		}

	    private void SuppressKeys(Process process)
	    {
	        _lowKeySuppressionService.HookSuppressKeysOnProcess(process, new[]
	        {
                Keys.Home, //Default Social Club Shortcut
	            Keys.Shift | Keys.Tab,
	            Keys.Alt | Keys.F4,
	            Keys.Alt | Keys.Enter
            });
	    }

	    private async void DuplicateProcessKiller(Process gameProcess)
		{
			while (!gameProcess.HasExited)
			{
				await Task.Delay(1000);

				var otherProcesses = Process.GetProcessesByName(Gta5ProcessName).Where(p => p.Id != gameProcess.Id).ToList();

				foreach (var otherProcess in otherProcesses)
				{
					_logger.Write("Killing duplicate process.");
					TryKillProcess(otherProcess);
				}
			}
		}

		private void TryKillProcess(Process process)
		{
			try
			{
				process.Kill();
			}
			catch (Exception ex)
			{
				_logger.Write(ex);
			}
		}

		private bool SetUpMod(Process gameProcess, string gamePath)
		{
			var injectionSuccess = _gameInjectionService.InjectIntoProcess(gameProcess);

			if (injectionSuccess)
				_gameInjectionService.InjectCustomAsiFiles(gameProcess, gamePath);

			return injectionSuccess;
		}

		private async Task<Process> WaitForGameStart()
		{
			_logger.Write("Waiting for game start.");

			var startTime = Environment.TickCount;

			while (Environment.TickCount - startTime < WaitForStartTimeout)
			{
				var gta5Process = Process.GetProcessesByName(Gta5ProcessName).FirstOrDefault();
				if (gta5Process != null)
				{
					_logger.Write("Game process detected. Waiting for main window.");

					await WaitForMainWindow(gta5Process);

					return gta5Process;
				}

				await Task.Delay(500);
			}
			
			_logger.Write("Game process not detected within timeout.", LogMessageSeverity.Error);
			_notificationService.ShowNotification("GTA 5 did not start within 5 minutes.", true);
			return null;
		}

	    private static async Task WaitForMainWindow(Process gameProcess)
	    {
	        while (gameProcess.MainWindowHandle == IntPtr.Zero)
	            await Task.Delay(100);
	    }

		private void LaunchGame(string gamePath)
		{
			_logger.Write("Launching game.");

			var launcherPath = Path.Combine(gamePath, "GTAVLauncher.exe");

			if (Directory.GetFiles(gamePath, "*.wow").Length != 0)
			{
				_logger.Write($"*.wow files detected, starting launcher: {launcherPath}");
				Process.Start(launcherPath);
				return;
			}

			var gtavExePath = Path.Combine(gamePath, Constants.GTA5ExecutableFileName);

			_logger.Write($"Reading GTA5.exe to determine launch method: {gtavExePath}");

            // Ommitted code to detect Steam/RockStar Warehouse version
		    var isSteamVersion = true;

			_logger.Write($"DRM ID value detected. Is steam version: {isSteamVersion}");

			Process.Start(isSteamVersion ? "steam://run/271590" : launcherPath);
		}
        
		private bool PatchStartup()
		{
			_logger.Write("Patching startup.");

			var profilePaths = GetProfilePaths();
			if (profilePaths == null || profilePaths.Length < 1)
			{
				_logger.Write("No profile paths found.", LogMessageSeverity.Error);
				return false;
			}

			try
			{
				foreach (var profilePath in profilePaths)
					PatchProfileStartup(profilePath);
			}
			catch (Exception e)
			{
				_logger.Write(e);
				return false;
			}
			
			return true;
		}

		private void PatchProfileStartup(string profilePath)
		{
			throw new NotImplementedException();
		}

		private bool ApplyGameSettings()
		{
            throw new NotImplementedException();
		}

		private static string[] GetProfilePaths()
		{
			var profilesFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Rockstar Games\GTA V\Profiles");
			var pcSettingsFilePaths = Directory.Exists(profilesFolderPath) ? Directory.GetFiles(profilesFolderPath, PcSettingsFileName, SearchOption.AllDirectories) : null;

			return pcSettingsFilePaths?.Select(Path.GetDirectoryName).ToArray();
		}

		private bool ApplyFixedSaveGame()
		{
			_logger.Write("Applying custom save game.");

			var profilePaths = GetProfilePaths();

			if (profilePaths == null || profilePaths.Length < 1)
			{
				_logger.Write("No profile paths detected.", LogMessageSeverity.Warning);
				MessageBox.Show("GTA 5 must have launched into single player at last once. Pleas do so, before launching GT-MP.");
				return false;
			}

			_logger.Write($"Profile paths:\n{string.Join("\n", profilePaths)}");

			try
			{
				foreach (var profilePath in profilePaths)
				{
					RestoreBackedUpSaveGame(profilePath);
					CopyFixedSaveGame(profilePath);
				}
			}
			catch (Exception ex)
			{
				_logger.Write(ex);
				_notificationService.ShowNotification("Failed to apply fixed saved game.", true);

				return false;
			}

			return true;

		}

		private void RestoreBackedUpSaveGame(string saveGameFolderPath)
		{
			var saveGameBackupFilePath = Path.Combine(saveGameFolderPath, "SGTA50000.bak");
			var saveGameFilePath = Path.Combine(saveGameFolderPath, "SGTA50000");

			if (!File.Exists(saveGameBackupFilePath)) return;
			if (File.Exists(saveGameFilePath))
			{
				_logger.Write($"Deleting save game file: {saveGameFilePath}");
				File.Delete(saveGameFilePath);
			}

			_logger.Write($"Moving backed up save game file: {saveGameBackupFilePath} to {saveGameFilePath}");
			FileOperationsHelper.MoveFileIgnoreReadonly(saveGameBackupFilePath, saveGameFilePath);
		}

		private void CopyFixedSaveGame(string saveGameFolderPath)
		{
			var currentSaveGameBackupFilePath = Path.Combine(saveGameFolderPath, "SGTA50000.bak");
			var currentSaveGameFilePath = Path.Combine(saveGameFolderPath, "SGTA50000");
			var fixedSaveGameFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"savegame\SGTA50000");

			if (File.Exists(currentSaveGameFilePath))
			{
				_logger.Write($"Backing up save game file: {currentSaveGameFilePath} to {currentSaveGameBackupFilePath}");
				FileOperationsHelper.MoveFileIgnoreReadonly(currentSaveGameFilePath, currentSaveGameBackupFilePath);
			}

			_logger.Write($"Copying custom save game: {fixedSaveGameFilePath} to {currentSaveGameFilePath}");
			File.Copy(fixedSaveGameFilePath, currentSaveGameFilePath);
		}

		private void MoveActiveModsToDisabledFolder(string gamePath)
		{
			var disabledModsFolderPath = Path.Combine(gamePath, DisabledModsFolderName);
			FileOperationsHelper.EnsureDirectory(disabledModsFolderPath);

			_logger.Write("Moving active game mods to disabled folder");
			try
			{
				MoveAsiFilesToDisabledFolder(gamePath, disabledModsFolderPath);
				MoveGrandTheftMultiplayerLibsToDisabledFolder(gamePath, disabledModsFolderPath);
			}
			catch (Exception e)
			{
				_logger.Write(e);
			}
		}

		private void MoveAsiFilesToDisabledFolder(string gamePath, string disabledModsFolderPath)
		{
			var asiModFiles = Directory.GetFiles(gamePath, "*.asi", SearchOption.TopDirectoryOnly);
			foreach (var asiModFilePath in asiModFiles)
			{
				var modFileName = Path.GetFileName(asiModFilePath);
				if (modFileName == null) continue;

				var targetDisabledPath = Path.Combine(disabledModsFolderPath, modFileName);

				_logger.Write($"Moving game mod from {asiModFilePath} to {targetDisabledPath}");
				FileOperationsHelper.MoveFileIgnoreReadonly(asiModFilePath, targetDisabledPath);
			}
		}

		private void MoveGrandTheftMultiplayerLibsToDisabledFolder(string gamePath, string disabledModsFolderPath)
		{
			foreach (var modFileName in _modFiles)
			{
				var modFileTargetPath = Path.Combine(gamePath, modFileName);
				var modFileDisabledPath = Path.Combine(disabledModsFolderPath, modFileName);

				if (!File.Exists(modFileTargetPath)) continue;

				_logger.Write($"Moving game mod from {modFileTargetPath} to {modFileDisabledPath}");
				FileOperationsHelper.MoveFileIgnoreReadonly(modFileTargetPath, modFileDisabledPath);
			}
		}

		private async Task<bool> CheckGta5Version()
		{
			var currentGameVersion = _gtaVersionProvider.GetCurrentGtaVersion();
			var supportedGameVersion = await _gtaVersionProvider.GetCurrentlySupportedGtaVersion();

			_logger.Write($"Current GTA version: {currentGameVersion}, supported GTA version: {supportedGameVersion}");

			if (currentGameVersion == null)
			{
				_notificationService.ShowNotification("Failed to get current GTA version.", true);
				return false;
			}

			if (supportedGameVersion == null)
			{
				_notificationService.ShowNotification("Failed to get supported GTA version.", true);
				return true;
			}

			if (currentGameVersion >= supportedGameVersion)
			{
				return true;
			}
			
			MessageBox.Show($"The supported GTA 5 version is {supportedGameVersion}, but you have {currentGameVersion} installed.\nThere may be issues launching GT-MP.");

			return true;
		}

		private async Task<bool> CheckRunningProcesses()
		{
			_logger.Write("Check running processes.");

			var blockingProcessesRunning = GetRunningBlockingProcesses().ToArray();

			_logger.Write($"Running blocking processes: {string.Join(", ", blockingProcessesRunning.Select(p => p.ProcessName))}");

			if (!blockingProcessesRunning.Any() || await ClosedBlockingProcesses()) return true;

			_notificationService.ShowNotification("Failed to launch game. Blocking processes are still running.", true);
			return false;
		}

		private bool CheckLaunchDependancies()
		{
			if (_launchDependencyCheckService.DependenciesAreInstalledProperly()) return true;

			_notificationService.ShowNotification("Failed to launch game. Missing dependency.", true);
			return false;
		}

		private async Task<bool> ClosedBlockingProcesses()
		{
            var userWantsToCloseRunningProcesses = PromptUserToCloseRunningGame();

            if (userWantsToCloseRunningProcesses == MessageBoxResult.Cancel)
            {
                _logger.Write("User chose not to close running processes and cancel.");
                return false;
            }

            if (userWantsToCloseRunningProcesses == MessageBoxResult.No)
			{
				_logger.Write("User chose not to close running processes.");
				return true;
			}

            _logger.Write("User chose to close running processes.");

            foreach (var process in GetRunningBlockingProcesses())
            {
                if (!await KillProcess(process))
                {
                    _logger.Write($"Failed to kill process: {process.ProcessName}");
                    return false;
                }
            }

		    await Task.Delay(5000);

			return !GetRunningBlockingProcesses().Any();
		}

		private async Task<bool> KillProcess(Process process)
		{
			try
			{
				_logger.Write($"Attempt to kill process {process.ProcessName}");
				process.Kill();
			}
			catch (Exception ex)
			{
				_logger.Write(ex);
				_notificationService.ShowNotification($"Failed to kill process: {process.ProcessName}.exe", true);
				return false;
			}

			await Task.Run(() => { process.WaitForExit(ProcessKillTimeout); });

			return process.HasExited;
		}

		private static MessageBoxResult PromptUserToCloseRunningGame()
		{
			return MessageBox.Show(
                "It seems that GTA V is already running. Would you like to forcibly close it, and launch anyways?",
				"GTA V already running",
				MessageBoxButton.YesNoCancel);
		}

		private IEnumerable<Process> GetRunningBlockingProcesses()
		{
			return _blockingProcesses.SelectMany(Process.GetProcessesByName);
		}

		public void ShutdownGrandTheftMultiplayer()
		{
			var gamePath = _gamePathProvider.GetGta5DirectoryPath();
			if (string.IsNullOrWhiteSpace(gamePath) || !Directory.Exists(gamePath)) return;

			_splashScreenProgressProvider.SetCurrentStep(SplashScreenProgressStep.RevertingFiles);

		    _socialClubCommandlineService.TryEnsureOfflineModeCommandLine(false);
			//_lowKeySuppressionService.UnHookSuppressKeysOnProcess();

			foreach (var profilePath in GetProfilePaths())
				RestoreBackedUpSaveGame(profilePath);
		}
	}
}