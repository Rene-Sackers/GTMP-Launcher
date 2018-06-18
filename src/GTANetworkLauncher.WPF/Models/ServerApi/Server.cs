using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Launcher.Helpers;
using GrandTheftMultiplayer.Launcher.Properties;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;

namespace GrandTheftMultiplayer.Launcher.Models.ServerApi
{
    public class Server : INotifyPropertyChanged
    {
        private readonly IServerQueryingService _serverQueryingService;
        private readonly int _uniqueId;
        
        private bool _isFavorited;
        private bool _isVerified;
        private string _name;
        private short _maxPlayers;
        private short _playerCount;
        private string _gameMode;
        private bool _lan;
        private bool _isPasswordProtected;
        private string _map;
        private bool _isQueried;
        private int _ping;
        private bool _isRecent;

        public event PropertyChangedEventHandler PropertyChanged;

        public string UniqueAddress { get; }

        public bool IsIpv6 { get; }

        public string Ip { get; set; }

        public int Port { get; set; }

        public bool IsFavorited
        {
            get => _isFavorited;
            set
            {
                if (value == _isFavorited) return;
                _isFavorited = value;
                OnPropertyChanged();
            }
        }

        public bool IsVerified
        {
            get => _isVerified;
            set
            {
                if (value == _isVerified) return;
                _isVerified = value;
                OnPropertyChanged();
            }
        }

        public bool IsQueried
        {
            get => _isQueried;
            set
            {
                if (value == _isQueried) return;
                _isQueried = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public short MaxPlayers
        {
            get => _maxPlayers;
            set
            {
                if (value == _maxPlayers) return;
                _maxPlayers = value;
                OnPropertyChanged();
            }
        }

        public short PlayerCount
        {
            get => _playerCount;
            set
            {
                if (value == _playerCount) return;
                _playerCount = value;
                OnPropertyChanged();
            }
        }

        public int Ping
        {
            get => _ping;
            set
            {
                if (value == _ping) return;
                _ping = value;
                OnPropertyChanged();
            }
        }

        public bool IsPasswordProtected
        {
            get => _isPasswordProtected;
            set
            {
                if (value == _isPasswordProtected) return;
                _isPasswordProtected = value;
                OnPropertyChanged();
            }
        }

        public string GameMode
        {
            get => _gameMode;
            set
            {
                if (value == _gameMode) return;
                _gameMode = value;
                OnPropertyChanged();
            }
        }

        public string Map
        {
            get => _map;
            set
            {
                if (value == _map) return;
                _map = value;
                OnPropertyChanged();
            }
        }

        public bool LAN
        {
            get => _lan;
            set
            {
                if (value == _lan) return;
                _lan = value;
                OnPropertyChanged();
            }
        }

        public bool IsRecent
        {
            get => _isRecent;
            set
            {
                if (value == _isRecent) return;
                _isRecent = value;
                OnPropertyChanged();
            }
        }
        
        public Server(string uniqueAddress, IServerQueryingService serverQueryingService)
        {
            _serverQueryingService = serverQueryingService;
            _uniqueId = uniqueAddress.GetHashCode();

            UniqueAddress = uniqueAddress;
            Name = uniqueAddress;

            var splitAddress = uniqueAddress.Split(':');
            if (splitAddress.Length < 2) return;

            // Supports IPv6, but LidGren doesn't.
            Ip = string.Join(":", splitAddress.Take(splitAddress.Length - 1));

            if (splitAddress.Length > 2)
                Ip = IPAddress.Parse(Ip).MapToIPv4().ToString();

            if (int.TryParse(splitAddress.Last(), out int port))
                Port = port;

            if (splitAddress.Length > 2)
                IsIpv6 = true;
        }

        public override int GetHashCode()
        {
            return _uniqueId;
        }

        public override bool Equals(object obj)
        {
            var otherServer = obj as Server;
            if (otherServer == null) return false;

            return otherServer.GetHashCode() == GetHashCode();
        }

        private static string CleanMessage(string message)
        {
            return Regex.Replace(message, @"(~.*?~|~|<|>|'|""|∑|\\|¦)", "");
        }

        public async Task UpdateInfo()
        {
            IsQueried = false;

            Ping = -1;
            
            var info = await _serverQueryingService.QueryServer(this);
            if (info == null) return;

            Ping = (int)await PingHelper.GetPingAsync(Ip);
            Name = CleanMessage(info.ServerName);
            MaxPlayers = info.MaxPlayers;
            PlayerCount = info.PlayerCount;
            IsPasswordProtected = info.PasswordProtected;
            GameMode = CleanMessage(info.Gamemode);
            LAN = info.LAN;
            Map = "None";
            
            IsQueried = true;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
