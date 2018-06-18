using System;
using GrandTheftMultiplayer.Launcher.Models.Logger;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;

namespace GrandTheftMultiplayer.Launcher.Services.Design
{
	public class DesignLogger : ILogger
	{
		public void Write(string message, LogMessageSeverity severity = LogMessageSeverity.Info)
		{
		}

		public void Write(object message, LogMessageSeverity severity = LogMessageSeverity.Info)
		{
		}

		public void Write(Exception exception, LogMessageSeverity severity = LogMessageSeverity.Info)
		{
		}
	}
}