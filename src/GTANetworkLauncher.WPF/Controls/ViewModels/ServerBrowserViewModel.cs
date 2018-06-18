using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GrandTheftMultiplayer.Launcher.Extensions;
using GrandTheftMultiplayer.Launcher.Helpers;
using GrandTheftMultiplayer.Launcher.Models.GameLauncherService;
using GrandTheftMultiplayer.Launcher.Models.Messaging.LauncherSplashScreen;
using GrandTheftMultiplayer.Launcher.Models.Notifications;
using GrandTheftMultiplayer.Launcher.Models.ServerApi;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;
using ServerModel = GrandTheftMultiplayer.Launcher.Models.ServerApi.Server;

namespace GrandTheftMultiplayer.Launcher.Controls.ViewModels
{
    public class ServerBrowserViewModel : ViewModelBase
    {
        // Services
        private readonly IServerListProvider _serverListProvider;
        private readonly INotificationService _notificationService;
        private readonly IServerQueryingService _serverQueryingService;
        private readonly TaskQueue _updateListTaskQueue = new TaskQueue();

        // Privates
        private bool _isServerListRefreshing;
        private string _serverFilterText = string.Empty;
        private ObservableCollection<ServerModel> _servers = new ObservableCollection<ServerModel>();
        private ServerModel _selectedServer;
        private bool _showPasswordedServers = true;
        private ServerCategory _serverCategoryFilter = ServerCategory.All;

        // Properties
        public Func<ServerModel, bool> ServerFilter { get; set; }

        public ObservableCollection<ServerModel> Servers
        {
            get => _servers;
            set
            {
                if (value == _servers) return;
                _servers = value;
                RaisePropertyChanged();
            }
        }

        public ServerModel SelectedServer
        {
            get => _selectedServer;
            set
            {
                if (Equals(value, _selectedServer)) return;
                _selectedServer = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowPasswordedServers
        {
            get => _showPasswordedServers;
            set
            {
                if (value == _showPasswordedServers) return;
                _showPasswordedServers = value;
                RaisePropertyChanged();
                LoadServers();
            }
        }

        public ServerCategory ServerCategoryFilter
        {
            get => _serverCategoryFilter;
            set
            {
                if (value == _serverCategoryFilter) return;
                _serverCategoryFilter = value;
                RaisePropertyChanged();
                LoadServers();
            }
        }

        // Commands
        public RelayCommand RefreshServersCommand { get; set; }

        public RelayCommand<string> SearchBoxTextChangedCommand { get; set; }

        public RelayCommand<ServerModel> JoinServerCommand { get; set; }

        public RelayCommand<ServerModel> ToggleFavoriteCommand { get; set; }

        public RelayCommand<ServerModel> CopyServerAddressCommand { get; set; }

        public RelayCommand<ServerModel> CopyServerPortCommand { get; set; }

        public RelayCommand CloseServerDetailPaneCommand { get; set; }

        public ServerBrowserViewModel(
            IServerListProvider serverListProvider,
            INotificationService notificationService,
            IServerQueryingService serverQueryingService)
        {
            _serverListProvider = serverListProvider;
            _notificationService = notificationService;
            _serverQueryingService = serverQueryingService;

            RefreshServersCommand = new RelayCommand(RefreshServers);
            SearchBoxTextChangedCommand = new RelayCommand<string>(SearchBoxTextChanged);
            JoinServerCommand = new RelayCommand<ServerModel>(JoinServer);
            ToggleFavoriteCommand = new RelayCommand<ServerModel>(ToggleFavorite);
            CopyServerAddressCommand = new RelayCommand<ServerModel>(CopyServerAddress);
            CloseServerDetailPaneCommand = new RelayCommand(CloseServerDetailPane);
            
            if (IsInDesignModeStatic) LoadDesignData();
        }

        private static void JoinServer(ServerModel server) =>
            Messenger.Default.Send(new LaunchGameMessage(GameLaunchMode.Multiplayer, server.Ip, server.Port));

        private void CloseServerDetailPane()
        {
            SelectedServer = null;
        }

        private bool SetClipboardText(string text)
        {
            try
            {
                Clipboard.SetText(text ?? string.Empty);
            }
            catch
            {
                _notificationService?.ShowNotification("Failed to copy to clipboard.", true);
                return false;
            }

            return true;
        }

        private void CopyServerAddress(ServerModel server)
        {
            if (SetClipboardText(server.UniqueAddress))
                _notificationService?.ShowNotification("Address copied to clipboard.", Notification.ShortMessageDelay);
        }

        private void ToggleFavorite(ServerModel server)
        {
            server.IsFavorited = !server.IsFavorited;
            _notificationService?.ShowNotification("Favorites", $"{server.Name} has been {(server.IsFavorited ? "added" : "removed")} from your favorites.", Notification.ShortMessageDelay);
        }

        private void SearchBoxTextChanged(string text)
        {
            _serverFilterText = text;

            // Todo: add timeout
            LoadServers();
        }

        private bool ApplyServerFilter(ServerModel server)
        {
            if (!ShowPasswordedServers && server.IsPasswordProtected) return false;

            if (string.IsNullOrWhiteSpace(_serverFilterText)) return true;

            // TODO: Filter server category
            // if (ServerCategoryFilter != ServerCategory.All && false) return false;

            return
                server.Name?.IndexOf(_serverFilterText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                server.GameMode?.IndexOf(_serverFilterText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                server.UniqueAddress?.IndexOf(_serverFilterText, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private async void LoadDesignData()
        {
            Servers = new ObservableCollection<ServerModel>(await SimpleIoc.Default.GetInstance<IServerListProvider>().GetServers());
            SelectedServer = Servers.FirstOrDefault(s => s.IsPasswordProtected);
        }

        public async Task LoadServers(bool refreshCache = false)
        {
            _isServerListRefreshing = true;
            try
            {
                await _serverQueryingService.ClearRefreshQueue();

                var servers = (await _serverListProvider.GetServers(refreshCache))
                    .Where(s => ServerFilter == null || ServerFilter(s))
                    .Where(ApplyServerFilter)
                    .ToList();

                await Servers.UpdateToTargetAsync(servers, _updateListTaskQueue);
            }
            catch
            {
                _notificationService.ShowNotification("Failed to fetch server list from GT-MP master server!", true);
            }
            _isServerListRefreshing = false;
        }

        private void RefreshServers()
        {
            if (!_isServerListRefreshing)
            {
                LoadServers(true);
                _notificationService?.ShowNotification("Server list", "Refreshed servers.", Notification.ShortMessageDelay);
                return;
            }
            _notificationService?.ShowNotification("Server list", "Already refreshing server list.", Notification.ShortMessageDelay, true);
        }
    }
}
