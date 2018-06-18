using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using GalaSoft.MvvmLight.Messaging;
using GrandTheftMultiplayer.Launcher.Models.GameLauncherService;
using GrandTheftMultiplayer.Launcher.Models.Messaging.LauncherSplashScreen;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;
using Microsoft.Win32;

namespace GrandTheftMultiplayer.Launcher.Services.Runtime
{
    public class ProtocolGameLauncherService : IProtocolGameLauncherService
    {
        private const string ProtocolName = "gtmp";
        
        private readonly ILogger _logger;

        public ProtocolGameLauncherService(ILogger logger)
        {
            _logger = logger;
        }

        public void CheckIfLaunchArgumentExists()
        {
            var launchCla = Environment.GetCommandLineArgs().FirstOrDefault(cla => cla.StartsWith($"{ProtocolName}://"));
            if (launchCla == null)
                return;

            ProcessLaunchCla(launchCla);
        }

        private void ProcessLaunchCla(string launchCla)
        {
            _logger.Write($"Found launch game argument: \"{launchCla}\"");

            var strippedCla = launchCla.Replace($"{ProtocolName}://", "").Trim('/');
            var splitCla = strippedCla.Split(':');

            if (splitCla.Length == 0)
            {
                _logger.Write("Launch argument invalid. Expected to be able to split with :, and have more than 1 result.");
                return;
            }

            if (splitCla.Length < 2 || !ushort.TryParse(splitCla[1], out ushort port))
            {
                _logger.Write("No, or faulty port in launch argument, using default port.");
                port = Constants.DefaultGamePort;
            }

            if (Uri.CheckHostName(splitCla[0]) == UriHostNameType.Unknown)
            {
                _logger.Write("Invalid IP or hostname specified.");
                return;
            }

            _logger.Write($"Launching game into server with hostname/port: {splitCla[0]}, {port}");
            Messenger.Default.Send(new LaunchGameMessage(GameLaunchMode.Multiplayer, splitCla[0], port));
        }

        public void TryVerifyProtocolRegistration()
        {
            try
            {
                VerifyProtocolRegistration();
                _logger.Write("Verified protocol registration.");
            }
            catch (Exception e)
            {
                _logger.Write("Failed to verify protocol registration.");
                _logger.Write(e);
            }
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        private static void VerifyProtocolRegistration()
        {
            var gtmpKey = Registry.ClassesRoot.CreateSubKey("gtmp", RegistryKeyPermissionCheck.Default);
            gtmpKey.SetValue(null, $"URL:{ProtocolName} protocol");
            gtmpKey.SetValue("URL Protocol", string.Empty);

            var iconKey = gtmpKey.CreateSubKey("DefaultIcon", RegistryKeyPermissionCheck.Default);
            iconKey.SetValue(null, Assembly.GetExecutingAssembly().Location);

            var shellKey = gtmpKey.CreateSubKey("Shell", RegistryKeyPermissionCheck.Default);
            var openKey = shellKey.CreateSubKey("Open", RegistryKeyPermissionCheck.Default);
            var commandKey = openKey.CreateSubKey("Command", RegistryKeyPermissionCheck.Default);
            commandKey.SetValue(null, "\"" + Assembly.GetExecutingAssembly().Location + "\" \"%1\"");
        }
    }
}