using System;
using System.IO;
using System.Xml.Serialization;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;
using GrandTheftMultiplayer.Shared;

namespace GrandTheftMultiplayer.Launcher.Services.Runtime
{
    public class SettingsProvider : ISettingsProvider
    {
	    private readonly ILogger _logger;
	    private readonly INotificationService _notificationService;
        private readonly Lazy<PlayerSettings> _settings;

        public event EventDelegates.SettingsSavedEventHandler SettingsSaved;

		public SettingsProvider(
			ILogger logger,
			INotificationService notificationService)
		{
			_logger = logger;
			_notificationService = notificationService;

            _settings = new Lazy<PlayerSettings>(LoadSettings);
        }

        private static string GetSettingsFilePath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.SettingsFileName);
        }

        public PlayerSettings GetCurrentSettings()
        {
            return _settings.Value;
        }

        private PlayerSettings LoadSettings()
        {
            _logger.Write("Loading settings file.");

            try
            {
                if (!File.Exists(GetSettingsFilePath()))
                    CreateNewSettingsFile();

                using (var settingsFileStream = File.OpenRead(GetSettingsFilePath()))
                {
                    return (PlayerSettings)GetSerializer().Deserialize(settingsFileStream);
                }
            }
            catch (Exception ex)
            {
                _logger.Write(ex);
                _notificationService.ShowNotification($"Failed to create settings file. Error message: {ex.Message}.\nMaybe try running as administrator?", true);
                // ignored
            }

            return new PlayerSettings();
        }

        public bool SaveSettings(PlayerSettings settings)
        {
			_logger.Write("Saving settings file.");

            try
            {
                using (var writeStream = new StreamWriter(GetSettingsFilePath()))
                {
                    var xmlSerializer = GetSerializer();
                    xmlSerializer.Serialize(writeStream, settings);
                    writeStream.Close();

					SettingsSaved?.Invoke(settings);
                    return true;
                }
            }
            catch (Exception ex)
			{
				_logger.Write(ex);
				_notificationService.ShowNotification("Settings", $"Failed to save settings. Error message: {ex.Message}", true);
            }

            return false;
        }

        private void CreateNewSettingsFile()
        {
            SaveSettings(new PlayerSettings());
        }

        private static XmlSerializer GetSerializer()
        {
            return new XmlSerializer(typeof(PlayerSettings));
        }
    }
}
