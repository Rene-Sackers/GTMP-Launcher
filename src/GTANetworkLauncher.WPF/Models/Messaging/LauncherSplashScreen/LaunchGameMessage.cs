using GalaSoft.MvvmLight.Messaging;
using GrandTheftMultiplayer.Launcher.Models.GameLauncherService;

namespace GrandTheftMultiplayer.Launcher.Models.Messaging.LauncherSplashScreen
{
	public class LaunchGameMessage : NotificationMessage
	{
	    public GameLaunchMode LaunchMode { get; }

        public bool JoinServer { get; }

	    public string JoinServerIp { get; }

	    public int JoinServerPort { get; }

	    public LaunchGameMessage(GameLaunchMode launchMode)
	        : base(nameof(LaunchGameMessage))
	    {
	        LaunchMode = launchMode;
	    }

	    public LaunchGameMessage(GameLaunchMode launchMode, string joinServerIp, int joinServerPort)
	        : base(nameof(LaunchGameMessage))
	    {
	        LaunchMode = launchMode;
	        JoinServerIp = joinServerIp;
	        JoinServerPort = joinServerPort;
	        JoinServer = true;
	    }
    }
}