using GalaSoft.MvvmLight.Messaging;
using GrandTheftMultiplayer.Launcher.Models.PromptWindow;

namespace GrandTheftMultiplayer.Launcher.Models.Messaging.MainWindow
{
    public class ShowEulaDialogResult : NotificationMessage<bool>
    {
        public ShowEulaDialogResult(bool result) : base(result, nameof(ShowEulaDialogResult))
        {
        }
    }
}