using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using GrandTheftMultiplayer.Launcher.Models.GameLauncherService;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;

namespace GrandTheftMultiplayer.Launcher.Services.Runtime
{
    public class LaunchDependencyCheckService : ILaunchDependencyCheckService
    {
        private readonly RequiredDependency[] _requiredRepDependencies =
        {
            new RequiredDependency(
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full",
                "Release",
                value => (int)value >= 379893,
                ".NET Framework ( required version: 4.5.2 or newer )",
                new List<string>())
            {
                DownloadUrl = "https://download.microsoft.com/download/D/5/C/D5C98AB0-35CC-45D9-9BA5-B18256BA2AE6/NDP462-KB3151802-Web.exe"
            },
            new RequiredDependency(
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\Installer\Dependencies\{f65db027-aff3-4070-886a-0d87064aabb1}",
                "Version",
                "Microsoft Visual C++ 2013 Redistributable (x86)",
                new List<string>()
                {
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\Installer\Dependencies\{e6e75766-da0f-4ba2-9788-6ea593ce702d}"
                })
            {
                DownloadUrl = "http://download.microsoft.com/download/2/E/6/2E61CFA4-993B-4DD4-91DA-3737CD5CD6E3/vcredist_x86.exe"
            },
            new RequiredDependency(
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\Installer\Dependencies\{050d4fc8-5d48-4b8f-8972-47c82c46020f}",
                "Version",
                "Microsoft Visual C++ 2013 Redistributable (x64)",
                new List<string>()
                {
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\Installer\Dependencies\{82f2609e-68ba-408d-963f-530ad8809435}",
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\Installer\Dependencies\{5d0723d3-cff7-4e07-8d0b-ada737deb5e6}"
                })
            {
                DownloadUrl = "http://download.microsoft.com/download/2/E/6/2E61CFA4-993B-4DD4-91DA-3737CD5CD6E3/vcredist_x64.exe"
            },
            new RequiredDependency(
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\Installer\Dependencies\{e2803110-78b3-4664-a479-3611a381656a}",
                "Version",
                "Microsoft Visual C++ 2015 Redistributable (x86)",
                new List<string>()
                {
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\Installer\Dependencies\,,x86,14.0,bundle",
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\Installer\Dependencies\{74d0e5db-b326-4dae-a6b2-445b9de1836e}",
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\Installer\Dependencies\{e2803110-78b3-4664-a479-3611a381656a}"
                })
            {
                DownloadUrl = "https://download.microsoft.com/download/9/3/F/93FCF1E7-E6A4-478B-96E7-D4B285925B00/vc_redist.x86.exe"
            },
            new RequiredDependency(
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\Installer\Dependencies\{d992c12e-cab2-426f-bde3-fb8c53950b0d}",
                "Version",
                "Microsoft Visual C++ 2015 Redistributable (x64)",
                new List<string>()
                {
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\Installer\Dependencies\,,amd64,14.0,bundle",
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\Installer\Dependencies\{e46eca4f-393b-40df-9f49-076faf788d83}"
                })

            {
                DownloadUrl = "https://download.microsoft.com/download/9/3/F/93FCF1E7-E6A4-478B-96E7-D4B285925B00/vc_redist.x64.exe"
            }
        };

        private readonly ILogger _logger;
        private readonly INotificationService _notificationService;

        public LaunchDependencyCheckService(
            ILogger logger,
            INotificationService notificationService)
        {
            _logger = logger;
            _notificationService = notificationService;
        }

        public bool DependenciesAreInstalledProperly()
        {
            _logger.Write("Checking launch dependencies.");

            if (!Environment.Is64BitOperatingSystem)
            {
                _logger.Write($"Is not 64-bit OS. OS version: {Environment.OSVersion}");
                _notificationService.ShowNotification("GT-MP requires a 64-bit operating system.", true);
                return false;
            }

            RequiredDependency firstMissingDependency = null;
            try
            {
                foreach (var dependency in _requiredRepDependencies)
                {
                    _logger.Write($"Check for dependency: {dependency.Name}");

                    if (dependency.IsInstalledCorrectly() || dependency.IsAnyEquivalentInstalledCorrectly()) continue;

                    if(dependency.Name.StartsWith("Microsoft Visual C++ 2015"))
                        continue;
                    firstMissingDependency = dependency;
                    break;
                }
            }
            catch (Exception ex)
            {
                _logger.Write(ex);
                var launchAnyways = AskToLaunchAnyways();

                if (!launchAnyways) return false;

                _logger.Write("User chose to launch anyways.");
                return true;
            }

            if (firstMissingDependency == null) return true;

            _logger.Write($"Missing dependency. ({firstMissingDependency.Name})");

            if (MessageBox.Show(
                    $"{firstMissingDependency.Name} is missing.\nWould you like to download it now?",
                    "Missing dependency",
                    MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes)
            {
                Process.Start(firstMissingDependency.DownloadUrl);
            }

            return false;
        }

        private static bool AskToLaunchAnyways()
        {
            return
                MessageBox.Show(
                    "Failed to check for .NET or Visual C++ dependency. Launch game anyways?",
                    "Dependency check failed",
                    MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes;
        }
    }
}