namespace GrandTheftMultiplayer.Launcher.Models.SplashScreenProgressProvider
{
	public enum SplashScreenProgressStep
	{
		Initializing,
		CheckingRunningProcesses,
		CheckingDependencies,
		CheckingCefVersion,
		CheckingClientVersion,
		DownloadNewClient,
		VerifyingGta5Path,
	    EnsuringGamePathRegistryKey,
		CheckingGta5Version,
		ApplyingCustomSaveGame,
		DisablingActiveMods,
		ApplyingGameSettings,
		PatchingGameStartup,
		CreatingCommandLineArgumentsFile,
		LaunchingMultiplayerGame,
		LaunchingSingleplayerGame,
		WaitingForGameToStart,
		InjectingGrandTheftMultiplayer,
		Done,
		RevertingFiles
	}
}