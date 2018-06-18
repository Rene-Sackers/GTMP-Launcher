using System;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;
using GrandTheftMultiplayer.Shared;

namespace GrandTheftMultiplayer.Launcher.Services.Design
{
    public class DesignSettingsProvider : ISettingsProvider
    {
        public event EventDelegates.SettingsSavedEventHandler SettingsSaved;

        public PlayerSettings GetCurrentSettings()
        {
            return new PlayerSettings
            {
                DisplayName = "Sample Display Name",
                GamePath = "C:\\Games\\Steam\\steamapps\\Grand Theft Auto V\\GTA5.exe",
                ServerHostPath = "C:\\GT-MP\\server",
                DarkTheme = false
            };
        }

        public bool SaveSettings(PlayerSettings settings)
        {
            throw new NotImplementedException();
        }
    }
}
