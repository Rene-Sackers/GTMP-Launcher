using GalaSoft.MvvmLight.Messaging;
using GrandTheftMultiplayer.Launcher.Models.Messaging.LauncherSplashScreen;

namespace GrandTheftMultiplayer.Launcher.Views
{
	/// <summary>
	/// Interaction logic for LauncherSplashScreen.xaml
	/// </summary>
	public partial class LauncherSplashScreen
	{
		public LauncherSplashScreen()
		{
			InitializeComponent();

            Messenger.Default.Register<ToggleLauncherSplashScreenVisibleMessage>(this, ToggleSplashScreenVisibleMessageReceived);
		}

		private void ToggleSplashScreenVisibleMessageReceived(ToggleLauncherSplashScreenVisibleMessage message)
		{
            if (message.Visible) Show();
			else Hide();
		}
	}
}
