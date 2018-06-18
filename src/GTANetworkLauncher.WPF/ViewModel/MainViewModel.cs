using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using System.Diagnostics;
using GrandTheftMultiplayer.Launcher.Controls.ViewModels;
using GrandTheftMultiplayer.Launcher.Helpers;
using GrandTheftMultiplayer.Launcher.Models.GameLauncherService;
using GrandTheftMultiplayer.Launcher.Models.Help;
using GrandTheftMultiplayer.Launcher.Models.Messaging.LauncherSplashScreen;
using GrandTheftMultiplayer.Launcher.Models.Messaging.MainWindow;
using GrandTheftMultiplayer.Launcher.Models.Messaging.UpdateSplashScreen;
using GrandTheftMultiplayer.Launcher.Models.News;
using GrandTheftMultiplayer.Launcher.Models.Notifications;
using GrandTheftMultiplayer.Launcher.Models.Statistics;
using GrandTheftMultiplayer.Launcher.Models.Status;
using GrandTheftMultiplayer.Launcher.Models.Troubleshoot;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Launcher.Models.PromptWindow;

namespace GrandTheftMultiplayer.Launcher.ViewModel
{
	public class MainViewModel : ViewModelBase
    {
        // Constants
        private const string DarkThemeUri = "pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseDark.xaml";
        private const string LightThemeUri = "pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml";
        private const int StatisticsUpdateTimeout = 10 * 1000;

        // Services
        private readonly ITroubleshootProvider _troubleshootProvider;
        private readonly INewsProvider _newsProvider;
        private readonly IHelpProvider _helpProvider;
        private readonly IServerListProvider _serverListProvider;
        private readonly IStatisticsProvider _statisticsProvider;
        private readonly IServerStatusProvider _serverStatusProvider;
        private readonly ISelfUpdateService _selfUpdateService;
		private readonly ISettingsProvider _settingsProvider;
        private readonly IServerHostProvider _serverHostProvider;
        private readonly ILogger _logger;

        public INotificationService NotificationService { get; set; }

        // Privates
        private bool _isAboutFlyoutOpen;
        private TroubleshootingDisplayModel _troubleshooting;
        private StatisticsResponse _statistics;
        private PlayerSettings _settings;
        private ResourceDictionary _darkThemeDictionary;
        private ResourceDictionary _lightThemeDictionary;

        private string _serverOutput;
        private Process _serverProcess;
        private bool _isShuttingDownServer;

        private List<ServerBrowserViewModel> _serverBrowsersPerformedInitialLoad = new List<ServerBrowserViewModel>();

        public bool IsRunning => _serverProcess != null && !_serverProcess.HasExited && !_isShuttingDownServer;

        public string ServerOutput
        {
            get { return _serverOutput; }
            set { _serverOutput = value; RaisePropertyChanged("ServerOutput"); }
        }

        // Commands
        public RelayCommand AboutCommand { get; set; }

        public RelayCommand LaunchButtonClickedCommand { get; set; }

        public RelayCommand BrowseGameFileCommand { get; set; }

        public RelayCommand ApplySettingsCommand { get; set; }

        public RelayCommand BrowseServerFileCommand { get; set; }

        public RelayCommand BrowseResourceDirectoryCommand { get; set; }
        public RelayCommand OpenServerSettingsXmlCommand { get; set; }

        public RelayCommand StartServerCommand { get; set; }

        public RelayCommand StopServerCommand { get; set; }

        public RelayCommand RestartServerCommand { get; set; }

        public RelayCommand<ServerBrowserViewModel> ServerListTabGotFocusCommand { get; set; }

        public RelayCommand<Notification> RemoveNotificationCommand { get; set; }

        // Server browsers
        public ServerBrowserViewModel ServerListViewModel { get; set; }

        public ServerBrowserViewModel VerifiedServerListViewModel { get; set; }

        public ServerBrowserViewModel FavoriteServerListViewModel { get; set; }

        public ServerBrowserViewModel LocalServerListViewModel { get; set; }

        public ServerBrowserViewModel RecentServerListViewModel { get; set; }

        // Others
        public ObservableCollection<HelpDisplayModel> HelpItems { get; set; } = new ObservableCollection<HelpDisplayModel>();
        
        public ObservableCollection<NewsDisplayModel> News { get; set; } = new ObservableCollection<NewsDisplayModel>();

        public ObservableCollection<ServerStatus> ServerStatus { get; set; } = new ObservableCollection<ServerStatus>();

        public PlayerSettings Settings => _settings ?? (_settings = _settingsProvider.GetCurrentSettings());

        public TroubleshootingDisplayModel Troubleshooting
        {
            get
            {
                return _troubleshooting;
            }
            set
            {
                if (value == _troubleshooting) return;

                _troubleshooting = value;
                RaisePropertyChanged();
            }
        }

        public StatisticsResponse Statistics
        {
            get
            {
                return _statistics;
            }
            set
            {
                if (value == _statistics) return;

                _statistics = value;
                RaisePropertyChanged();
            }
        }

        public bool IsAboutFlyoutOpen
        {
            get { return _isAboutFlyoutOpen; }
            set
            {
                _isAboutFlyoutOpen = value;
                RaisePropertyChanged();
            }
        }

        private void LoadTheme(bool dark)
        {
            return;
            var darkThemeLoaded = _darkThemeDictionary != null && Application.Current.Resources.MergedDictionaries.Contains(_darkThemeDictionary);
            var lightThemeLoaded = _lightThemeDictionary != null && Application.Current.Resources.MergedDictionaries.Contains(_lightThemeDictionary);

            if (dark)
            {
                if (lightThemeLoaded)
                {
                    Application.Current.Resources.MergedDictionaries.Remove(_lightThemeDictionary);
                }
                if (darkThemeLoaded) return;

                _darkThemeDictionary = new ResourceDictionary { Source = new Uri(DarkThemeUri) };
                Application.Current.Resources.MergedDictionaries.Add(_darkThemeDictionary);
                return;
            }

            if (darkThemeLoaded)
            {
                Application.Current.Resources.MergedDictionaries.Remove(_darkThemeDictionary);
            }
            if (lightThemeLoaded) return;

            _lightThemeDictionary = new ResourceDictionary { Source = new Uri(LightThemeUri) };
            Application.Current.Resources.MergedDictionaries.Add(_lightThemeDictionary);
        }

        public MainViewModel(
            ILogger logger,
            ISettingsProvider settingsProvider,
            INewsProvider newsProvider,
            IHelpProvider helpProvider,
            IServerListProvider serverListProvider,
            IStatisticsProvider statisticsProvider,
            IServerStatusProvider serverStatusProvider,
			INotificationService notificationService,
			ISelfUpdateService selfUpdateService,
            IGameBackupService gameBackupService,
            IServerHostProvider serverHostProvider,
            ITroubleshootProvider troubleshootProvider,
            IProtocolGameLauncherService protocolGameLauncherService)
        {
            _logger = logger;
            _settingsProvider = settingsProvider;
            _newsProvider = newsProvider;
            _helpProvider = helpProvider;
            _serverListProvider = serverListProvider;
            _statisticsProvider = statisticsProvider;
            _serverStatusProvider = serverStatusProvider;
			_selfUpdateService = selfUpdateService;
            _serverHostProvider = serverHostProvider;
            _troubleshootProvider = troubleshootProvider;
			NotificationService = notificationService;
            
            LoadTheme(_settingsProvider.GetCurrentSettings().DarkTheme);

            AboutCommand = new RelayCommand(AboutButtonClicked);
            LaunchButtonClickedCommand = new RelayCommand(LaunchButtonClicked);
            BrowseGameFileCommand = new RelayCommand(BrowseGameFileClicked);
            BrowseServerFileCommand = new RelayCommand(BrowseServerFileClicked);
            ApplySettingsCommand = new RelayCommand(ApplySettingsClicked);
            BrowseResourceDirectoryCommand = new RelayCommand(BrowseResourceDirectoryClicked);
            OpenServerSettingsXmlCommand = new RelayCommand(EditServerSettingsXmlClicked);
            StartServerCommand = new RelayCommand(StartServerClicked);
            StopServerCommand = new RelayCommand(StopServerClicked);
            RestartServerCommand = new RelayCommand(RestartServerClicked);
            RemoveNotificationCommand = new RelayCommand<Notification>(RemoveNotificationClicked);
            ServerListTabGotFocusCommand = new RelayCommand<ServerBrowserViewModel>(ServerListTabGotFocus);

            ServerListViewModel = SimpleIoc.Default.GetInstanceWithoutCaching<ServerBrowserViewModel>();

            VerifiedServerListViewModel = SimpleIoc.Default.GetInstanceWithoutCaching<ServerBrowserViewModel>();
            VerifiedServerListViewModel.ServerFilter = s => s.IsVerified;

            FavoriteServerListViewModel = SimpleIoc.Default.GetInstanceWithoutCaching<ServerBrowserViewModel>();
            FavoriteServerListViewModel.ServerFilter = s => s.IsFavorited;

            LocalServerListViewModel = SimpleIoc.Default.GetInstanceWithoutCaching<ServerBrowserViewModel>();
            LocalServerListViewModel.ServerFilter = s => s.LAN;

            RecentServerListViewModel = SimpleIoc.Default.GetInstanceWithoutCaching<ServerBrowserViewModel>();
            RecentServerListViewModel.ServerFilter = s => s.IsRecent;

            _serverListProvider.FavoritedServersUpdated += ServerListProviderOnFavoritedServersUpdated;
			_settingsProvider.SettingsSaved += SettingsSaved;

            CheckForUpdate();
			GetNews();
            GetTroubleshooting();
            GetHelpItems();
            GetStatistics();
            GetServerStatus();

            if (_settingsProvider.GetCurrentSettings().AskToBackUpGame)
                gameBackupService.CheckBackupStatus();

            protocolGameLauncherService.CheckIfLaunchArgumentExists();
            protocolGameLauncherService.TryVerifyProtocolRegistration();

            EnsureEula();
        }

        private void EnsureEula()
        {
            if (_settingsProvider.GetCurrentSettings().AcceptedEula) return;

            Messenger.Default.Register<ShowEulaDialogResult>(this, message =>
            {
                if (!message.Content)
                {
                    _logger.Write(@"User declined EULA ¯\_(ツ)_/¯");
                    Application.Current.Shutdown();
                    return;
                }

                var settings = _settingsProvider.GetCurrentSettings();
                settings.AcceptedEula = true;
                _settingsProvider.SaveSettings(settings);
                _logger.Write("User accepted EULA ( ͡° ͜ʖ ͡°)");
            });

            Messenger.Default.Send(new ShowEulaDialog());
        }

        private void ServerListTabGotFocus(ServerBrowserViewModel serverBrowserViewModel)
        {
            if (_serverBrowsersPerformedInitialLoad.Contains(serverBrowserViewModel)) return;

            _serverBrowsersPerformedInitialLoad.Add(serverBrowserViewModel);
            serverBrowserViewModel.LoadServers();
        }

        private async void CheckForUpdate()
	    {
		    if (!await _selfUpdateService.IsUpdateAvailable()) return;
			if (!ShouldUpdate()) return;

			Messenger.Default.Send(new PerformUpdateMessage());
	    }

	    private static bool ShouldUpdate()
	    {
		    return MessageBox.Show("There's a new update available of the launcher. Install it now?", "Update available", MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes;
	    }

	    private void SettingsSaved(PlayerSettings settings)
		{
			_settings = settings;
			RaisePropertyChanged(nameof(Settings));
		}

		private void RemoveNotificationClicked(Notification notification)
        {
            NotificationService.RemoveNotificationIfExists(notification);
        }

        private async void ServerListProviderOnFavoritedServersUpdated()
        {
            if (Settings != null)
            {
                Settings.FavoriteServers = (await _serverListProvider.GetServers())?.Where(s => s.IsFavorited).Select(s => s.UniqueAddress).ToList();
                _settingsProvider.SaveSettings(Settings);
            }
            
            FavoriteServerListViewModel.LoadServers();
        }

        private void ApplySettingsClicked()
        {
            if (Settings.DarkTheme != _settingsProvider.GetCurrentSettings().DarkTheme)
            {
                LoadTheme(Settings.DarkTheme);
            }

            if (_settingsProvider.SaveSettings(Settings))
                NotificationService.ShowNotification("Settings", "Saved!", Notification.ShortMessageDelay);
        }

        private void BrowseGameFileClicked()
        {
	        var selectedFile = FileOperationsHelper.BrowseForGta5Exe();

            if (!File.Exists(selectedFile))
            {
                NotificationService.ShowNotification("Invalid file selected.", true);
                return;
            }

            Settings.GamePath = selectedFile;
            RaisePropertyChanged(nameof(Settings));
        }

        private void BrowseServerFileClicked()
        {
            var selectedFile = FileOperationsHelper.BrowseForGtMpServerExe();

            if (!File.Exists(selectedFile))
            {
                NotificationService.ShowNotification("Invalid file selected.", true);
                return;
            }

            Settings.ServerHostPath = selectedFile;
            RaisePropertyChanged(nameof(Settings));
        }

        private void BrowseResourceDirectoryClicked()
        {
            if (!File.Exists(Settings.ServerHostPath))
            {
                NotificationService.ShowNotification("Improper or no GT-MP server directory selected.", true);
                return;
            }

            ServerHostingHelper.OpenResourcesDirectory(Settings.ServerHostPath);
        }

        private void EditServerSettingsXmlClicked()
        {
            if (!File.Exists(Settings.ServerHostPath))
            {
                NotificationService.ShowNotification("Improper or no GT-MP server directory selected.", true);
                return;
            }

            if (!ServerHostingHelper.OpenSettingsFile(Settings.ServerHostPath))
            {
                NotificationService.ShowNotification("settings.xml file does not exist.", true);
                return;
            }
        }

        private void StartServerClicked()
        {
            if (!File.Exists(Settings.ServerHostPath))
            {
                NotificationService.ShowNotification("Improper or no GT-MP server directory selected.", true);
                return;
            }

            if (IsRunning) return;

            ServerOutput = "";

            string serverDirectory = Path.GetDirectoryName(Settings.ServerHostPath);

            _serverProcess = new Process
            {
                StartInfo = new ProcessStartInfo(Settings.ServerHostPath)
                {
                    WorkingDirectory = serverDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = false,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            _serverProcess.Start();

            Task.Factory.StartNew(ServerOutputStreamReader);
        }

        private async void StopServerClicked()
        {
            await StopServer();
        }

        private async Task StopServer()
        {
            if (!File.Exists(Settings.ServerHostPath))
            {
                NotificationService.ShowNotification("Improper or no GT-MP server directory selected.", true);
                return;
            }

            if (!IsRunning || _isShuttingDownServer) return;

            _isShuttingDownServer = true;

            ServerOutput += "Sending shutdown event." + Environment.NewLine;

            await ServerHostingHelper.ShutDownConsoleProcess(_serverProcess);

            if (_serverProcess == null) return;
            if (!_serverProcess.HasExited) _serverProcess.Kill();

            _serverProcess.Close();
            _serverProcess.Dispose();
            _serverProcess = null;

            RaisePropertyChanged(nameof(IsRunning));

            ServerOutput += "Server stopped." + Environment.NewLine;

            _isShuttingDownServer = false;
        }

        private async void RestartServerClicked()
        {
            await StopServer();
            StartServerClicked();
        }

        private async void ServerOutputStreamReader()
        {
            while (_serverProcess != null && !_serverProcess.HasExited)
            {
                try
                {
                    var output = await _serverProcess.StandardOutput.ReadLineAsync();
                    ServerOutput += output + Environment.NewLine;
                }
                catch
                {
                    return;
                }
            }
        }

        private void AboutButtonClicked()
        {
            IsAboutFlyoutOpen = !IsAboutFlyoutOpen;
        }

        private async void GetNews()
        {
            var newsItems = await _newsProvider.GetNewsAsync();
            if (newsItems == null) return;

            News.Clear();
            newsItems.ToList().ForEach(n => News.Add(new NewsDisplayModel(n)));
            //News.First().ReadMoreExpanded = true;
        }

        private async void GetTroubleshooting()
        {
            var troubleshootPost = await _troubleshootProvider.GetTroubleshootingAsync();
            if (troubleshootPost == null) return;

            Troubleshooting = new TroubleshootingDisplayModel(troubleshootPost);
        }

        private void GetHelpItems()
        {
            var helpItems = _helpProvider.GetHelpItems();
            if (helpItems == null) return;

            HelpItems.Clear();
            helpItems.ToList().ForEach(h => HelpItems.Add(new HelpDisplayModel(h)));
        }

        private async void GetStatistics()
        {
            while (true)
            {
                Statistics = await _statisticsProvider.GetStatistics();
                await Task.Delay(StatisticsUpdateTimeout);
            }
        }

        private async void GetServerStatus()
        {
            var serverStatus = await _serverStatusProvider.GetServerStatusList();
            if (serverStatus == null) return;
            serverStatus.Data.ForEach(ServerStatus.Add);
        }

        private void LaunchButtonClicked()
        {
            Messenger.Default.Send(new LaunchGameMessage(GameLaunchMode.Multiplayer));
        }
    }
}