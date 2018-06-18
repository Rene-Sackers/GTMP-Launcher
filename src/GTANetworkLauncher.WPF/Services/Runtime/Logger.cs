using System;
using System.IO;
using System.Text;
using System.Windows;
using GrandTheftMultiplayer.Launcher.Helpers;
using GrandTheftMultiplayer.Launcher.Models.Logger;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;

namespace GrandTheftMultiplayer.Launcher.Services.Runtime
{
	public class Logger : ILogger, IDisposable
	{
		private const string LogFilePath = "logs/launcher.log";
		private const int LogFileMaxSizeInMegabytes = 5;
		private readonly TaskQueue _taskQueue = new TaskQueue(1, false);

		private StreamWriter _logFileStream;

		public Logger()
		{
			var logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LogFilePath);

			DeleteLogFileIfExceedsMaxSize(logFilePath);
			EnsureLogFile(logFilePath);
			OpenLogFileStream(logFilePath);
		}

		private static void EnsureLogFile(string logFilePath)
		{
			FileOperationsHelper.EnsureFileDirectory(logFilePath);

			if (File.Exists(logFilePath)) return;

			try
			{
				File.Create(logFilePath).Dispose();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Failed to create log file! Exception:\n\n" + ex);
			}
		}

		private static void DeleteLogFileIfExceedsMaxSize(string logFilePath)
		{
			if (!File.Exists(logFilePath)) return;

			var fileSize = new FileInfo(logFilePath).Length;
			var fileSizeInMegabytes = fileSize / 1024 / 1024;

			if (fileSizeInMegabytes < LogFileMaxSizeInMegabytes) return;

			try
			{
				File.Delete(logFilePath);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Failed to delete log file exceeding max file size. Exception:\n\n" + ex);
			}
		}

		private void OpenLogFileStream(string logFilePath)
		{
			if (!File.Exists(logFilePath)) return;

			try
			{
				_logFileStream = new StreamWriter(logFilePath, true, Encoding.UTF8) {AutoFlush = true};
			}
			catch (Exception ex)
			{
				MessageBox.Show("Failed to open log file for writing. Exception:\n\n" + ex);
			}
		}

		private async void TryWriteLogLine(string message, LogMessageSeverity severity)
		{
			if (_logFileStream == null) return;

			message = FormatMessage(message, severity);

			try
			{
				await _taskQueue.Enqueue(() => _logFileStream.WriteLineAsync(message));
			}
			catch
			{
				// ignore
			}
		}

		private static string FormatMessage(string message, LogMessageSeverity severity)
		{
			return $"[{DateTime.Now.ToLongTimeString()}] [{severity}] {message}";
		}

		private static string GetExceptionText(Exception exception)
		{
			var exceptionText = string.Empty;

			exceptionText += $"Message: {exception.Message}. Exception: {exception}\n";

			if (exception.InnerException != null)
				exceptionText += GetExceptionText(exception.InnerException);

			return exceptionText;
		}

		public void Write(string message, LogMessageSeverity severity = LogMessageSeverity.Info)
		{
			TryWriteLogLine(message, severity);
		}

		public void Write(object message, LogMessageSeverity severity = LogMessageSeverity.Info)
		{
			TryWriteLogLine(message.ToString(), severity);
		}

		public void Write(Exception exception, LogMessageSeverity severity = LogMessageSeverity.Error)
		{
			TryWriteLogLine(GetExceptionText(exception), severity);
		}

		public void Dispose()
		{
			_logFileStream?.Dispose();
		}
	}
}