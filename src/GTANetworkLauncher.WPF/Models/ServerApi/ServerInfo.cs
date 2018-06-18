using Newtonsoft.Json;

namespace GrandTheftMultiplayer.Launcher.Models.ServerApi
{
    public class ServerInfo
    {
        public int Port { get; set; }

        public short MaxPlayers { get; set; }

        public string ServerName { get; set; }

        public short CurrentPlayers { get; set; }

        public string GameMode { get; set; }

        [JsonProperty("ip")]
        public string IpPortAddress { get; set; }

        public bool Passworded { get; set; }

        public string Fqdn { get; set; }

        public string ServerVersion { get; set; }

        public override int GetHashCode()
        {
            return IpPortAddress.GetHashCode();
        }
    }
}