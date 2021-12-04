using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtgDev.Utils.Extensions
{
    public static class ZipFileExntesions
    {
        public static async Task ExtractToDirectoryAsync(string sourceArchiveFileName, string destinationDirectoryName)
        {
            using ZipArchive archive = ZipFile.OpenRead(sourceArchiveFileName);
            foreach (var entry in archive.Entries)
            {
                var fullName = entry.FullName;
                string path = Path.GetFullPath(Path.Combine(destinationDirectoryName, fullName));

                bool isDirectory = string.IsNullOrEmpty(entry.Name) &&
                    !string.IsNullOrEmpty(fullName) &&
                    fullName[^1] == '/' ||
                    fullName[^1] == '\\';
                if (isDirectory)
                {
                    Directory.CreateDirectory(path);
                }
                else
                {
                    await using Stream inStream = entry.Open();
                    await using Stream outStream = File.Create(path);
                    await ProcessStreams(inStream, outStream);
                }
            }
        }

        private static async Task ProcessStreams(Stream input, Stream output)
        {
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            do
            {
                bytesRead = await input.ReadAsync(buffer, 0, buffer.Length);
                await output.WriteAsync(buffer, 0, bytesRead);
            } while (bytesRead > 0);
        }
    }
}
