using System;
using GrandTheftMultiplayer.Launcher.Models.SplashScreenProgressProvider;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;

namespace GrandTheftMultiplayer.Launcher.Services.Design
{
	public class DesignSplashScreenProgressProvider : ISplashScreenProgressProvider
	{
		public event EventDelegates.LaunchStatusChangedHandler LaunchStatusChanged
		{
			add
			{
				value.Invoke("Example step", 45);
			}
			remove { }
		}

		public void SetCurrentStep(SplashScreenProgressStep progressStep)
		{
			throw new NotImplementedException();
		}
	}
}