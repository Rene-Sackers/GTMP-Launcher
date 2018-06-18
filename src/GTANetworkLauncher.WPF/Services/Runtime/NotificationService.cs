using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Launcher.Models.Notifications;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;

namespace GrandTheftMultiplayer.Launcher.Services.Runtime
{
    public class NotificationService : INotificationService
	{
        public ObservableCollection<Notification> Notifications { get; set; } = new ObservableCollection<Notification>();

        public async void ShowNotification(string title, string message, int? timeout, bool isErrorNotification)
        {
            var notification = new Notification(title, message, isErrorNotification);
            lock (Notifications)
            {
                Notifications.Add(notification);
            }

            if (!timeout.HasValue) return;

			await Task.Delay(timeout.Value * 1000);

			RemoveNotificationIfExists(notification);
		}

        public void RemoveNotificationIfExists(Notification notification)
        {
            lock (Notifications)
            {
                if (!Notifications.Contains(notification)) return;
                Notifications.Remove(notification);
            }
        }

        public void ShowNotification(string title, string message, bool isErrorNotification = false)
        {
            ShowNotification(title, message, null, isErrorNotification);
        }

        public void ShowNotification(string title, string message, int timeout)
        {
            ShowNotification(title, message, timeout, false);
        }

        public void ShowNotification(string message, int? timeout, bool isErrorNotification = false)
        {
            ShowNotification(null, message, timeout, isErrorNotification);
        }

        public void ShowNotification(string message, bool isErrorNotification)
        {
            ShowNotification(null, message, isErrorNotification);
        }

        public void ShowNotification(string message)
        {
            ShowNotification(null, message);
        }
    }
}
