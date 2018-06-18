using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace GrandTheftMultiplayer.Launcher
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            // ReSharper disable once UnusedVariable
            var mutex = new Mutex(true, "GrandTheftMultiplayerLauncher", out bool createdNew);

            if (!createdNew)
            {
                ShowWindow();
                Application.Current.Shutdown();
                return;
            }

            AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
            App.Main();
        }

        private static Assembly OnResolveAssembly(object sender, ResolveEventArgs args)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var assemblyName = new AssemblyName(args.Name);

            var path = assemblyName.Name + ".dll";
            if (assemblyName.CultureInfo.Equals(CultureInfo.InvariantCulture) == false)
            {
                path = $@"{assemblyName.CultureInfo}\{path}";
            }

            using (var stream = executingAssembly.GetManifestResourceStream(path))
            {
                if (stream == null)
                    return null;

                var assemblyRawBytes = new byte[stream.Length];
                stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);
                return Assembly.Load(assemblyRawBytes);
            }
        }

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private static void ShowWindow()
        {
            foreach (var process in Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName))
            {
                if (process.MainWindowHandle == IntPtr.Zero) continue;

                ShowWindow(process.MainWindowHandle, 1);
                SetForegroundWindow(process.MainWindowHandle);
                return;
            }
        }
    }
}
