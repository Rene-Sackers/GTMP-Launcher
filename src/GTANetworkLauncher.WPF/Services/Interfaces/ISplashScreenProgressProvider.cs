using GrandTheftMultiplayer.Launcher.Models.SplashScreenProgressProvider;

namespace GrandTheftMultiplayer.Launcher.Services.Interfaces
{
	public interface ISplashScreenProgressProvider
	{
		event EventDelegates.LaunchStatusChangedHandler LaunchStatusChanged;

		void SetCurrentStep(SplashScreenProgressStep progressStep);
	}
}