namespace GrandTheftMultiplayer.Launcher.Models.Notifications
{
    public class Notification
    {
        public const int ShortMessageDelay = 5;

        public string Title { get; }

        public string Message { get; }

        public bool IsErrorNotification { get; }

        public Notification(string title, string message, bool isErrorNotification = false)
        {
            Title = title;
            Message = message;
            IsErrorNotification = isErrorNotification;
        }
    }
}
