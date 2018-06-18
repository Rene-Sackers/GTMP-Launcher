using GalaSoft.MvvmLight.Messaging;
using GrandTheftMultiplayer.Launcher.ViewModel;

namespace GrandTheftMultiplayer.Launcher.Models.Messaging.MainWindow
{
    public class ShowEulaDialog : NotificationMessage
    {
        public ShowEulaDialog() : base(nameof(ShowEulaDialog))
        {
        }
    }
}