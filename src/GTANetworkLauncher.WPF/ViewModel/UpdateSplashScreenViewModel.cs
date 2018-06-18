using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GrandTheftMultiplayer.Launcher.Models.Messaging.UpdateSplashScreen;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;

namespace GrandTheftMultiplayer.Launcher.ViewModel
{
	public class UpdateSplashScreenViewModel : ViewModelBase
	{
		private readonly ISelfUpdateService _selfUpdateService;

		public double UpdateProgressPercentage { get; private set; }

		public UpdateSplashScreenViewModel(ISelfUpdateService selfUpdateService)
		{
			_selfUpdateService = selfUpdateService;
			_selfUpdateService.UpdateStatusChanged += UpdateStatusChanged;

			Messenger.Default.Register<PerformUpdateMessage>(this, PerformUpdateMessageReceived);
		}

		private void UpdateStatusChanged(double progressPercentage)
		{
			UpdateProgressPercentage = progressPercentage;
			RaisePropertyChanged(nameof(UpdateProgressPercentage));
		}

		private async void PerformUpdateMessageReceived(PerformUpdateMessage message)
		{
			Messenger.Default.Send(new ToggleUpdateSplashScreenVisibleMessage(true));

			await _selfUpdateService.UpdateSelf();

			Messenger.Default.Send(new ToggleUpdateSplashScreenVisibleMessage(false));
		}
	}
}