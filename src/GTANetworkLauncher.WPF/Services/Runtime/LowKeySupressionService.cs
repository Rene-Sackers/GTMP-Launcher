using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;

namespace GrandTheftMultiplayer.Launcher.Services.Runtime
{
    public class LowKeySuppressionService : ILowKeySuppressionService
    {
        private readonly ILogger _logger;

        private Keys[] _supressedKeys;

        [StructLayout(LayoutKind.Sequential)]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
        private struct KBDLLHOOKSTRUCT
        {
            public Keys key;
            public int scanCode;
            public int flags;
            public int time;
            public IntPtr extra;
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int id, LowLevelKeyboardProc callback, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hook);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hook, int nCode, IntPtr wp, IntPtr lp);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string name);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]

        private static extern short GetAsyncKeyState(Keys key);
        private IntPtr _windowsHook;
        private LowLevelKeyboardProc _keyboardProcess;

        public LowKeySuppressionService(ILogger logger)
        {
            _logger = logger;
        }

        public void HookSuppressKeysOnProcess(Process process)
        {
            var currentModule = process.MainModule;
            _keyboardProcess = OnKeyCaptured;
            _windowsHook = SetWindowsHookEx(13, _keyboardProcess, GetModuleHandle(currentModule.ModuleName), 0);
            _logger.Write($"Process {process.ProcessName} hooked for key supression");
        }

        public void HookSuppressKeysOnProcess(Process process, Keys[] supressedKeys)
        {
            HookSuppressKeysOnProcess(process);
            SetSupressedKeys(supressedKeys);
        }

        public void UnHookSuppressKeysOnProcess()
        {
            UnhookWindowsHookEx(_windowsHook);
            _windowsHook = IntPtr.Zero;
        }

        public void SetSupressedKeys(Keys[] supressedKeys)
        {
            _supressedKeys = supressedKeys;
        }

        private IntPtr OnKeyCaptured(int nCode, IntPtr wp, IntPtr lp)
        {
            if (nCode >= 0)
            {
                var keyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lp, typeof(KBDLLHOOKSTRUCT));
                if (_supressedKeys.Any(key => (key & Keys.KeyCode) == keyInfo.key && CheckModifier(key)))
                {
                    return (IntPtr)1;
                }
            }
            return CallNextHookEx(_windowsHook, nCode, wp, lp);
        }

        private static bool CheckModifier(Keys key)
        {
            if ((key & Keys.Control) == Keys.Control &&
                GetAsyncKeyState(Keys.LControlKey) == 0 && GetAsyncKeyState(Keys.RControlKey) == 0) return false;
            if ((key & Keys.Shift) == Keys.Shift &&
                GetAsyncKeyState(Keys.LShiftKey) == 0 && GetAsyncKeyState(Keys.RShiftKey) == 0) return false;
            if ((key & Keys.Alt) == Keys.Alt &&
                GetAsyncKeyState(Keys.LMenu) == 0 && GetAsyncKeyState(Keys.RMenu) == 0) return false;
            return true;
        }
    }
}
