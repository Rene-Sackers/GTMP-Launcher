using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GrandTheftMultiplayer.Launcher.Models.GameLauncherService;
using GrandTheftMultiplayer.Launcher.Models.Messaging.LauncherSplashScreen;
using GrandTheftMultiplayer.Launcher.Models.Notifications;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;

namespace GrandTheftMultiplayer.Launcher.ViewModel
{
	public class LauncherSplashScreenViewModel : ViewModelBase
	{
		private readonly INotificationService _notificationService;
		private readonly IGameLauncherService _gameLauncherService;

		private bool _isWatitingForShutdown;
		private Process _currentProcess;
		
		public string CurrentStatusText { get; private set; }

		public int CurrentStatusProgressPercentage { get; private set; }

		public LauncherSplashScreenViewModel(
			INotificationService notificationService,
			IGameLauncherService gameLauncherService,
			ISplashScreenProgressProvider splashScreenProgressProvider)
		{
			_notificationService = notificationService;
			_gameLauncherService = gameLauncherService;

			splashScreenProgressProvider.LaunchStatusChanged += LaunchStatusChanged;
			
			MessengerInstance.Register<LaunchGameMessage>(this, LaunchGameMessageReceived);
		}

		private void LaunchStatusChanged(string text, int progressPercentage)
		{
			CurrentStatusText = text;
			CurrentStatusProgressPercentage = progressPercentage;
			RaisePropertyChanged(nameof(CurrentStatusText));
			RaisePropertyChanged(nameof(CurrentStatusProgressPercentage));
		}

		private async void LaunchGameMessageReceived(LaunchGameMessage launchGameMessage)
		{
			if (_gameLauncherService.IsLaunching)
			{
				_notificationService.ShowNotification("Already launching.", Notification.ShortMessageDelay, true);
				return;
			}

			if (_isWatitingForShutdown)
			{
				var forceClose = AskToForceClose();
				if (!forceClose) return;

				_currentProcess.Kill();
				await WaitForProcessShutdown(_currentProcess);
			}

			Messenger.Default.Send(new ToggleLauncherSplashScreenVisibleMessage(true));

            if (launchGameMessage.JoinServer)
                _currentProcess = await _gameLauncherService.LaunchGameAndJoinServer(launchGameMessage.JoinServerIp, launchGameMessage.JoinServerPort);
            else
                _currentProcess = await _gameLauncherService.LaunchGame(launchGameMessage.LaunchMode);

			Messenger.Default.Send(new ToggleLauncherSplashScreenVisibleMessage(false));

			if (_currentProcess == null) return;

			_isWatitingForShutdown = true;
			await WaitForProcessShutdown(_currentProcess);

			Messenger.Default.Send(new ToggleLauncherSplashScreenVisibleMessage(true));

			if(launchGameMessage.LaunchMode == GameLaunchMode.Multiplayer)
				_gameLauncherService.ShutdownGrandTheftMultiplayer();

			Messenger.Default.Send(new ToggleLauncherSplashScreenVisibleMessage(false));

			_isWatitingForShutdown = false;
		}

		private static bool AskToForceClose()
		{
			return MessageBox.Show("Game currently running. Would you like to forcibly close it?", "Game already running", MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes;
		}

		private static Task WaitForProcessShutdown(Process process)
		{
			return Task.Run(() => process.WaitForExit());
		}
	}
}