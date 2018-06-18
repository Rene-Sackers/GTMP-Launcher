namespace GrandTheftMultiplayer.Launcher.Services.Interfaces
{
    public interface ISocialClubCommandlineService
    {
        bool ToggleCommandlineParameter(string commandLineParameter, bool enable);
        bool TryEnsureOfflineModeCommandLine(bool useOfflineMode);
    }
}