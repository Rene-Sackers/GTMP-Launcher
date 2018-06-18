using GalaSoft.MvvmLight.Messaging;

namespace GrandTheftMultiplayer.Launcher.Models.Messaging.UpdateSplashScreen
{
	public class PerformUpdateMessage : NotificationMessage
	{
		public PerformUpdateMessage() : base(nameof(PerformUpdateMessage))
		{
			
		}
	}
}