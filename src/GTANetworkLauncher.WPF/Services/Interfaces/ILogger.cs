using System;
using GrandTheftMultiplayer.Launcher.Models.Logger;

namespace GrandTheftMultiplayer.Launcher.Services.Interfaces
{
	public interface ILogger
	{
		void Write(string message, LogMessageSeverity severity = LogMessageSeverity.Info);

		void Write(object message, LogMessageSeverity severity = LogMessageSeverity.Info);

		void Write(Exception exception, LogMessageSeverity severity = LogMessageSeverity.Error);
	}
}