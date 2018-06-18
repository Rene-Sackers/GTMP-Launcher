using System.Collections.ObjectModel;
using GrandTheftMultiplayer.Launcher.Models.Notifications;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;

namespace GrandTheftMultiplayer.Launcher.Services.Design
{
	public class DesignNotificationService : INotificationService
	{
		public ObservableCollection<Notification> Notifications { get; set; } = new ObservableCollection<Notification>
		{
			new Notification("Sample error notification.", "Sample error notification text.", true),
			new Notification("Sample ok notification.", "Sample ok notification text.")
		};

		public void RemoveNotificationIfExists(Notification notification)
		{
		}

		public void ShowNotification(string message)
		{
		}

		public void ShowNotification(string message, bool isErrorNotification)
		{
		}

		public void ShowNotification(string message, int? timeout, bool isErrorNotification = false)
		{
		}

		public void ShowNotification(string title, string message, bool isErrorNotification = false)
		{
		}

		public void ShowNotification(string title, string message, int timeout)
		{
		}

		public void ShowNotification(string title, string message, int? timeout, bool isErrorNotification)
		{
		}
	}
}