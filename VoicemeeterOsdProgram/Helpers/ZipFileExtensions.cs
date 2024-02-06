using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AtgDev.Utils.ZipFileExtensions;

public static class ZipFileExtension
{
    public static async Task ExtractToDirectoryAsync(
        string sourceArchiveFileName, string destinationDirectoryName,
        IProgress<double> fileProg = null, IProgress<double> totalProg = null,
        CancellationToken cancellationToken = default)
    {
        using ZipArchive archive = ZipFile.OpenRead(sourceArchiveFileName);

        long totalBytes = archive.Entries.Sum(el => el.Length);
        long currentBytes = 0;

        foreach (var entry in archive.Entries)
        {
            var fullName = entry.FullName;
            string path = Path.GetFullPath(Path.Combine(destinationDirectoryName, fullName));

            if (IsDirectory(entry))
            {
                Directory.CreateDirectory(path);
            }
            else
            {
                await using Stream inStream = entry.Open();
                await using Stream outStream = File.Create(path);
                currentBytes = await ProcessStreams(inStream, outStream, 
                    entry.Length, currentBytes, totalBytes, 
                    fileProg, totalProg, cancellationToken);
            }
        }
    }

    private static async Task<long> ProcessStreams(
        Stream input, Stream output, 
        long inputLength, long currentBytes, long totalBytes,
        IProgress<double> fileProg = null, IProgress<double> totalProg = null, 
        CancellationToken cancellationToken = default)
    {
        var buffer = new byte[4096];
        long streamBytesRead = 0;
        int bytesRead;
        fileProg?.Report(0);
        while ((bytesRead = await input.ReadAsync(buffer, cancellationToken)) > 0)
        {
            await output.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);

            streamBytesRead += bytesRead;
            currentBytes += bytesRead;
            fileProg?.Report(streamBytesRead * 100.0 / inputLength);
            totalProg?.Report(currentBytes * 100.0 / totalBytes);
        }
        return currentBytes;
    }

    private static bool IsDirectory(ZipArchiveEntry entry)
    {
        return string.IsNullOrEmpty(entry.Name) &&
            !string.IsNullOrEmpty(entry.FullName) &&
            entry.FullName[^1] == '/' ||
            entry.FullName[^1] == '\\';
    }
}
