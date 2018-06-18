using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace GrandTheftMultiplayer.Launcher.Helpers
{
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public class ConsoleHandlingImports
	{
		internal const int CTRL_C_EVENT = 0;

		[DllImport("kernel32.dll")]
		internal static extern bool GenerateConsoleCtrlEvent(uint dwCtrlEvent, uint dwProcessGroupId);
		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool AttachConsole(uint dwProcessId);
		[DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
		internal static extern bool FreeConsole();
		[DllImport("kernel32.dll")]
		internal static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate handlerRoutine, bool add);

		internal delegate bool ConsoleCtrlDelegate(uint ctrlType);
	}
}
