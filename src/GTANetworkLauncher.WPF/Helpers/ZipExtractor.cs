using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Launcher.Extensions;

namespace GrandTheftMultiplayer.Launcher.Helpers
{
    public static class ZipExtractor
    {
        public static Task ExtractToDirectoryAsync(string zipFilePath, string targetDirectoryPath, bool overwrite)
        {
            return Task.Run(() => ExtractToDirectory(zipFilePath, targetDirectoryPath, overwrite));
        }

        private static void ExtractToDirectory(string zipFilePath, string targetDirectoryPath, bool overwrite)
        {
            using (var fileStream = File.OpenRead(zipFilePath))
            using (var archive = new ZipArchive(fileStream))
                archive.ExtractToDirectory(targetDirectoryPath, overwrite);
        }
    }
}