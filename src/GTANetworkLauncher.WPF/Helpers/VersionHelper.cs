using System;
using System.Diagnostics;
using System.IO;

namespace GrandTheftMultiplayer.Launcher.Helpers
{
	public static class VersionHelper
	{
		public static Version VersionFromFile(string filePath)
		{
			if (!File.Exists(filePath)) return null;

			var fileVersion = FileVersionInfo.GetVersionInfo(filePath);
			return Version.Parse(fileVersion.ProductVersion);
		}
	}
}