using GrandTheftMultiplayer.Shared;

namespace GrandTheftMultiplayer.Launcher.Services.Interfaces
{
    public interface ISettingsProvider
    {
	    event EventDelegates.SettingsSavedEventHandler SettingsSaved;

        PlayerSettings GetCurrentSettings();

        bool SaveSettings(PlayerSettings settings);
    }
}