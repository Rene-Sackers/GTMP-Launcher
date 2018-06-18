using System;
using System.IO;
using System.IO.Compression;

namespace GrandTheftMultiplayer.Launcher.Extensions
{
    public static class ZipArchiveExtensions
    {
        public static void ExtractToDirectory(this ZipArchive archive, string destinationDirectoryName, bool overwrite)
        {
            if (!overwrite)
            {
                archive.ExtractToDirectory(destinationDirectoryName);
                return;
            }

            foreach (var compressedFile in archive.Entries)
            {
                var compressedFileTargetPath = Path.Combine(destinationDirectoryName, compressedFile.FullName);
                var directory = Path.GetDirectoryName(compressedFileTargetPath);

                if (directory == null)
                    throw new InvalidOperationException($"Failed to get directory name of file {compressedFileTargetPath}");

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                if (compressedFile.Name != "")
                    compressedFile.ExtractToFile(compressedFileTargetPath, true);
            }
        }
    }
}
