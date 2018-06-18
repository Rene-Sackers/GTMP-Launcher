using GalaSoft.MvvmLight.Messaging;

namespace GrandTheftMultiplayer.Launcher.Models.Messaging.UpdateSplashScreen
{
	public class ToggleUpdateSplashScreenVisibleMessage : NotificationMessage
	{
		public bool Visible { get; }

		public ToggleUpdateSplashScreenVisibleMessage(bool visible)
			: base(nameof(ToggleUpdateSplashScreenVisibleMessage))
		{
			Visible = visible;
		}
	}
}