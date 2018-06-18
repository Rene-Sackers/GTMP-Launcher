using System.Diagnostics;
using System.Windows.Forms;

namespace GrandTheftMultiplayer.Launcher.Services.Interfaces
{
    public interface ILowKeySuppressionService
    {
        void HookSuppressKeysOnProcess(Process process);
        void HookSuppressKeysOnProcess(Process process, Keys[] supressedKeys);
        void SetSupressedKeys(Keys[] supressedKeys);
        void UnHookSuppressKeysOnProcess();
    }
}