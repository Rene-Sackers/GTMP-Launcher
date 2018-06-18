using GalaSoft.MvvmLight.Messaging;

namespace GrandTheftMultiplayer.Launcher.Models.Messaging.LauncherSplashScreen
{
	public class ToggleLauncherSplashScreenVisibleMessage : NotificationMessage
	{
		public bool Visible { get; }

		public ToggleLauncherSplashScreenVisibleMessage(bool visible)
			: base(nameof(ToggleLauncherSplashScreenVisibleMessage))
		{
			Visible = visible;
		}
	}
}