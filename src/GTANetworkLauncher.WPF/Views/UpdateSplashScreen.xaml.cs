using GalaSoft.MvvmLight.Messaging;
using GrandTheftMultiplayer.Launcher.Models.Messaging.UpdateSplashScreen;

namespace GrandTheftMultiplayer.Launcher.Views
{
	/// <summary>
	/// Interaction logic for UpdateSplashScreen.xaml
	/// </summary>
	public partial class UpdateSplashScreen
	{
		public UpdateSplashScreen()
		{
			InitializeComponent();

			Messenger.Default.Register<ToggleUpdateSplashScreenVisibleMessage>(this, ToggleUpdateSplashScreenVisibleMessageReceived);
		}

		private void ToggleUpdateSplashScreenVisibleMessageReceived(ToggleUpdateSplashScreenVisibleMessage message)
		{
			if (message.Visible) Show();
			else Hide();
		}
	}
}
