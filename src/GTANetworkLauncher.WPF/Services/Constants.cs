namespace GrandTheftMultiplayer.Launcher.Services
{
    public class Constants
    {
		// Defaults
	    private const string MasterServerAddress = "http://.../";
	    private const string StatusServerAddress = "http://.../";
		private const string DefaultUpdateChannel = "stable";

        public const int DefaultGamePort = 4499;

        // Generic
        public const string GTA5ExecutableFileName = "GTA5.exe";
        public const string SteamApiDllFileName = "steam_api64.dll";
        public const string ServerExeFileName = "GrandTheftMultiplayer.Server.exe";
        public const string ForumsApiKey = "LAUNCHER_NEWS_API_KEY";
        public const string SettingsFileName = "settings.xml";

        // API URLs
        //public const string NewsApi = MasterServerAddress + "welcome.json";
        public const string ServerApiUrl = MasterServerAddress + "servers/detailed";
        public const string StatisticsUrl = MasterServerAddress + "stats/masterlist";
        public const string VerfiedServersUrl = MasterServerAddress + "servers/verified";
        public const string ServerStatusUrl = StatusServerAddress + "api/v1/components";
        public const string ForumsApiUrl = "https://...";

        // GTA Backup Mirror
        //public const string BackupMirrorUrl = "https://...";

        // Formatted API URLs
        private const string SupportedGta5Version = MasterServerAddress + "update/{0}/game/version";
        private const string CEFVersion = MasterServerAddress + "update/{0}/cef/version";
		private const string LauncherVersion = MasterServerAddress + "update/{0}/launcher/version";
		private const string LauncherFileDownload = MasterServerAddress + "update/{0}/launcher/files";
		private const string ClientVersion = MasterServerAddress + "update/{0}/client/version";
		private const string UpdateFileDownload = MasterServerAddress + "update/{0}/client/files";

        public static string GetSupportedGta5VersionUrl(string updateChannel = DefaultUpdateChannel) => string.Format(SupportedGta5Version, updateChannel);

        public static string GetCEFVersionUrl(string updateChannel = DefaultUpdateChannel) => string.Format(CEFVersion, updateChannel);

	    public static string GetLauncherVersionUrl(string updateChannel = DefaultUpdateChannel) => string.Format(LauncherVersion, updateChannel);

		public static string GetLauncherFileDownloadUrl(string updateChannel = DefaultUpdateChannel) => string.Format(LauncherFileDownload, updateChannel);

		public static string GetClientVersionUrl(string updateChannel = DefaultUpdateChannel) => string.Format(ClientVersion, updateChannel);

		public static string GetUpdateFileDownloadUrl(string updateChannel = DefaultUpdateChannel) => string.Format(UpdateFileDownload, updateChannel);
    }
}
