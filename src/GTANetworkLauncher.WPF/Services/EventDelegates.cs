using GrandTheftMultiplayer.Shared;

namespace GrandTheftMultiplayer.Launcher.Services
{
    public static class EventDelegates
    {
        public delegate void FavoritedServersUpdatedHandler();

	    public delegate void LaunchStatusChangedHandler(string text, int progressPercentage);

		public delegate void SettingsSavedEventHandler(PlayerSettings settings);

	    public delegate void UpdateStatusChangedHandler(double progressPercentage);
    }
}
