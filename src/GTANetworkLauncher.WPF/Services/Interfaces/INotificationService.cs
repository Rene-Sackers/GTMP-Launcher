using System.Collections.ObjectModel;
using GrandTheftMultiplayer.Launcher.Models.Notifications;

namespace GrandTheftMultiplayer.Launcher.Services.Interfaces
{
	public interface INotificationService
	{
		ObservableCollection<Notification> Notifications { get; set; }

		void RemoveNotificationIfExists(Notification notification);

		void ShowNotification(string message);

		void ShowNotification(string message, bool isErrorNotification);

		void ShowNotification(string message, int? timeout, bool isErrorNotification = false);

		void ShowNotification(string title, string message, bool isErrorNotification = false);

		void ShowNotification(string title, string message, int timeout);

		void ShowNotification(string title, string message, int? timeout, bool isErrorNotification);
	}
}