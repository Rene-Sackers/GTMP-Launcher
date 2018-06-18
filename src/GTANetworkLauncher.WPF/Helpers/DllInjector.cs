using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using GrandTheftMultiplayer.Launcher.Models.DllInjectionService;

namespace GrandTheftMultiplayer.Launcher.Helpers
{
	public class DllInjector
	{
		private static readonly IntPtr IntptrZero = (IntPtr)0;

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr OpenProcess(uint dwDesiredAccess, int bInheritHandle, uint dwProcessId);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern int CloseHandle(IntPtr hObject);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr GetModuleHandle(string lpModuleName);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flAllocationType, uint flProtect);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] buffer, uint size, int lpNumberOfBytesWritten);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttribute, IntPtr dwStackSize, IntPtr lpStartAddress,
			IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

		public static DllInjectionResult Inject(Process process, string dllPath)
		{
			if (!File.Exists(dllPath))
			{
				return DllInjectionResult.DllNotFound;
			}

			if (!PerformInjection((uint)process.Id, dllPath))
			{
				return DllInjectionResult.InjectionFailed;
			}

			return DllInjectionResult.Success;
		}

		private static bool PerformInjection(uint targetProcess, string dllPath)
		{
			var hndProc = OpenProcess(0x2 | 0x8 | 0x10 | 0x20 | 0x400, 1, targetProcess);

			if (hndProc == IntptrZero)
			{
				return false;
			}

			var lpLlAddress = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");

			if (lpLlAddress == IntptrZero)
			{
				return false;
			}

			var lpAddress = VirtualAllocEx(hndProc, (IntPtr)null, (IntPtr)dllPath.Length, (0x1000 | 0x2000), 0X40);

			if (lpAddress == IntptrZero)
			{
				return false;
			}

			var bytes = Encoding.ASCII.GetBytes(dllPath);

			if (WriteProcessMemory(hndProc, lpAddress, bytes, (uint)bytes.Length, 0) == 0)
			{
				return false;
			}

			if (CreateRemoteThread(hndProc, (IntPtr)null, IntptrZero, lpLlAddress, lpAddress, 0, (IntPtr)null) == IntptrZero)
			{
				return false;
			}

			CloseHandle(hndProc);

			return true;
		}
	}
}