using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Launcher.Services;

namespace GrandTheftMultiplayer.Launcher.Helpers
{
    public class ServerHostingHelper
    {
        public static void OpenResourcesDirectory(string serverPath)
        {
            var serverDirectory = Path.GetDirectoryName(serverPath);

            if (string.IsNullOrEmpty(serverDirectory))
                return;

            var serverResourcesPath = Path.Combine(serverDirectory, "resources");
            Process.Start(serverResourcesPath);
        }

        public static bool OpenSettingsFile(string serverPath)
        {
            var serverDirectory = Path.GetDirectoryName(serverPath);

            try
            {
                if (serverDirectory == null)
                    return false;
                Process.Start(Path.Combine(serverDirectory, Constants.SettingsFileName));
            }
            catch (FileNotFoundException)
            {
                return false;
            }

            return true;
        }

        public static async Task ShutDownConsoleProcess(Process process)
        {
            if (!ConsoleHandlingImports.AttachConsole((uint) process.Id))
                return;

            ConsoleHandlingImports.SetConsoleCtrlHandler(null, true);
            try
            {
                ConsoleHandlingImports.GenerateConsoleCtrlEvent(ConsoleHandlingImports.CTRL_C_EVENT, 0);
                await Task.Factory.StartNew(process.WaitForExit);
            }
            finally
            {
                ConsoleHandlingImports.FreeConsole();
                ConsoleHandlingImports.SetConsoleCtrlHandler(null, false);
            }
        }
    }
}
