using System;
using System.IO;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;

namespace GrandTheftMultiplayer.Launcher.Services.Runtime
{
    public class SocialClubCommandlineService : ISocialClubCommandlineService
    {
        private const string OfflineModeCommandLineParameter = "-scOfflineOnly";
        private const string CommandLineFilePath = @"bin\commandline.txt";

        private readonly ILogger _logger;
        private readonly INotificationService _notificationService;

        public SocialClubCommandlineService(
            ILogger logger,
            INotificationService notificationService)
        {
            _logger = logger;
            _notificationService = notificationService;
        }

        public bool TryEnsureOfflineModeCommandLine(bool useOfflineMode)
        {
            _logger.Write($"Ensuring offline command line. (Offline Mode = {useOfflineMode})");
            return ToggleCommandlineParameter(OfflineModeCommandLineParameter, useOfflineMode);
        }

        public bool ToggleCommandlineParameter(string commandLineParameter, bool enable)
        {
            try
            {
                _logger.Write($"Toggling commandline parameter '{commandLineParameter}' ({enable})");

                var commandLineFilePath = GetCommandLineFilePath();
                
                if (!File.Exists(commandLineFilePath))
                {
                    if (!enable) return true;

                    File.AppendAllText(commandLineFilePath, commandLineParameter);
                    return true;
                }

                var fileContents = File.ReadAllText(commandLineFilePath);
                var hasParameterAlready = fileContents.Contains(commandLineParameter);

                if (enable && hasParameterAlready || !enable && !hasParameterAlready) return true;

                if (enable)
                    File.AppendAllText(commandLineFilePath, @" " + commandLineParameter);
                else
                    File.WriteAllText(commandLineFilePath, fileContents.Replace(" " + commandLineParameter, string.Empty).Replace(commandLineParameter, string.Empty));
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.Write(ex);
                _notificationService.ShowNotification($"Toggling commandline parameter '{commandLineParameter}' failed.", true);
                return false;
            }
        }

        private static string GetCommandLineFilePath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CommandLineFilePath);
        }
    }
}