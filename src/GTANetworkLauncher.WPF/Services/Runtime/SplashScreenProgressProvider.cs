using System.Collections.Generic;
using GrandTheftMultiplayer.Launcher.Models.SplashScreenProgressProvider;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;

namespace GrandTheftMultiplayer.Launcher.Services.Runtime
{
	public class SplashScreenProgressProvider : ISplashScreenProgressProvider
	{
		private class StepInfo
		{
			public string Text { get; }

			public int ProgressPercentage { get; }

			public StepInfo(string text, int progressPercentage)
			{
				Text = text;
				ProgressPercentage = progressPercentage;
			}
		}

		public event EventDelegates.LaunchStatusChangedHandler LaunchStatusChanged;

		private static readonly Dictionary<SplashScreenProgressStep, StepInfo> StepInfos = new Dictionary<SplashScreenProgressStep, StepInfo>
			{
				{SplashScreenProgressStep.Initializing, new StepInfo("Initializing", 0)},
				{SplashScreenProgressStep.CheckingRunningProcesses, new StepInfo("Checking running processes", 5)},
				{SplashScreenProgressStep.CheckingDependencies, new StepInfo("Checking dependencies", 10)},
				{SplashScreenProgressStep.CheckingCefVersion, new StepInfo("Checking CEF version", 15)},
				{SplashScreenProgressStep.CheckingClientVersion, new StepInfo("Checking client version", 20)},
				{SplashScreenProgressStep.DownloadNewClient, new StepInfo("Downloading new client", 25)},
				{SplashScreenProgressStep.VerifyingGta5Path, new StepInfo("Verifying GTA 5 path", 30)},
				{SplashScreenProgressStep.EnsuringGamePathRegistryKey, new StepInfo("Ensuring game path registry key", 32)},
                {SplashScreenProgressStep.CheckingGta5Version, new StepInfo("Checking GTA 5 version", 35)},
				{SplashScreenProgressStep.ApplyingCustomSaveGame, new StepInfo("Applying custom save game", 40)},
				{SplashScreenProgressStep.DisablingActiveMods, new StepInfo("Disabling active mods", 45)},
				{SplashScreenProgressStep.ApplyingGameSettings, new StepInfo("Applying game settings", 50)},
				{SplashScreenProgressStep.PatchingGameStartup, new StepInfo("Patching startup", 55)},
				{SplashScreenProgressStep.CreatingCommandLineArgumentsFile, new StepInfo("Creating command line arguments file", 60)},
				{SplashScreenProgressStep.LaunchingMultiplayerGame, new StepInfo("Launching Multiplayer", 65)},
				{SplashScreenProgressStep.LaunchingSingleplayerGame, new StepInfo("Launching Singleplayer", 65)},
				{SplashScreenProgressStep.WaitingForGameToStart, new StepInfo("Waiting for game to start", 80)},
				{SplashScreenProgressStep.InjectingGrandTheftMultiplayer, new StepInfo("Injecting GT-MP", 90)},
				{SplashScreenProgressStep.Done, new StepInfo("Done!", 100)},
				{SplashScreenProgressStep.RevertingFiles, new StepInfo("Reverting files", 100)},
			};

		public void SetCurrentStep(SplashScreenProgressStep progressStep)
		{
			var info = StepInfos[progressStep];

			LaunchStatusChanged?.Invoke(info.Text, info.ProgressPercentage);
		}
	}
}