using Octokit;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoicemeeterOsdProgram.Updater.Types;
using System.Runtime.InteropServices;
using AtgDev.Utils.DirectoryInfoExtensions;
using AtgDev.Utils.StreamExtensions;
using AtgDev.Utils.ZipFileExtensions;

namespace VoicemeeterOsdProgram.Updater
{
    public static class UpdateManager
    {
        private const string Owner = "A-tG";
        private const string RepoName = "VoicemeeterFancyOSD";
        private const string ExtractedFolder = "VoicemeeterFancyOSD";
        private const string BackupFolderName = $".{RepoName}UpdBak";

        private static Assembly m_assembly = Assembly.GetEntryAssembly();
        private static HttpClient m_httpClient = new();
        private static GitHubClient m_ghClient = new(new ProductHeaderValue(m_assembly.GetName().Name));
        private static ReleaseAsset m_latestAsset;
        private static CancellationTokenSource m_cts = new();

        public static Release LatestRelease
        {
            get;
            private set;
        }

        public static Version LatestVersion
        {
            get;
            private set;
        }

        public static OSPlatform DefaultOS { get; set; }

        public static Version CurrentVersion
        {
            get => m_assembly.GetName().Version;
        }

        public static string RepoUrl
        {
            get => $"www.github.com/{Owner}/{RepoName}";
        }

        public static async Task<UpdaterResult> TryCheckForUpdatesAsync()
        {
            UpdaterResult result = default;

            try
            {
                var releases = await m_ghClient.Repository.Release.GetAll(Owner, RepoName);
                if (releases.Count == 0) return UpdaterResult.ReleasesNotFound;

                var rel = releases[0];

                var latestVer = new Version(FilterVersionString(rel.TagName));
                m_latestAsset = rel.Assets.First((el) => IsArchitectureMatch(el.Name));

                result = latestVer > CurrentVersion ? UpdaterResult.NewVersionFound : UpdaterResult.VersionUpToDate;

                LatestVersion = latestVer;
                LatestRelease = rel;
#if DEBUG // to be able download older version when debugging
                return UpdaterResult.NewVersionFound;
#endif
            }
            catch (Exception e)
            {
                if (e is ApiException) result = UpdaterResult.ConnectionError;
                if (e is HttpRequestException) result = UpdaterResult.ConnectionError;
                if (e is InvalidOperationException) result = UpdaterResult.ArchitectureNotFound;

                m_latestAsset = null;
            }

            return result;
        }

        public static async Task<UpdaterResult> TryUpdateAsync(
            IProgress<CurrentTotalBytes> downloadProgress = null, 
            IProgress<double> extractionProgress = null, 
            IProgress<double> copyProgress = null)
        {
            var result = await TryCheckForUpdatesAsync();
            if (result != UpdaterResult.NewVersionFound) return result;

            var downloadRes = await TryDownloadAsync(m_latestAsset.BrowserDownloadUrl, m_latestAsset.Name, downloadProgress);
            if ((downloadRes.Res != UpdaterResult.Downloaded) || string.IsNullOrEmpty(downloadRes.Path)) return downloadRes.Res;

            var path = downloadRes.Path;
            var updateFolder = Path.GetDirectoryName(path);
            result = await TryUnzipAsync(path, extractionProgress);
            if (result == UpdaterResult.Unpacked)
            {
                if (TryCopyAndRestart(updateFolder, copyProgress))
                {
                    return UpdaterResult.Updated;
                }
                else
                {
                    return UpdaterResult.UpdateFailed;
                }
            }
            else
            {
                TryDeleteFolder(updateFolder);
                return result;
            }
        }

        public static bool TryDeleteBackup() => TryDeleteFolder(BackupFolderName);

        public static void CancelUpdate()
        {
            m_cts.Cancel();
            m_cts.Dispose();
            m_cts = new();
        }

        private static bool TryCopyAndRestart(string updateFolder, IProgress<double> copyProgress = null)
        {
            try
            {
                string copyTo = Path.TrimEndingDirectorySeparator(AppDomain.CurrentDomain.BaseDirectory);
                string copyFrom = updateFolder + @$"\{ExtractedFolder}";
                string program = Environment.ProcessPath;

                // just in case
                bool isValidPaths = (Directory.GetParent(updateFolder).ToString() == copyTo) &&
                    (Directory.GetParent(program).ToString() == copyTo);
                if (string.IsNullOrEmpty(program) || !isValidPaths) return false;

                if (!TryOvewriteFiles(copyFrom, copyTo, copyProgress))
                {
                    TryDeleteBackup();
                    return false;
                }

                TryDeleteFolder(updateFolder);

                Process.Start(program, ArgsHandler.AfterUpdateArg);

                return true;
            }
            catch { }
            return false;
        }

        private static bool TryOvewriteFiles(string fromFolder, string toFolder, IProgress<double> copyProgress = null)
        {
            bool result = false;
            try
            {
                DirectoryInfo fromDir, toDir, bakDir;
                fromDir = new(fromFolder);
                toDir = new(toFolder);
                bakDir = Directory.CreateDirectory(Path.Combine(toDir.ToString(), BackupFolderName));

                ulong totalSize, readSize = 0;
                totalSize = fromDir.GetSize();

                foreach (var file in fromDir.GetFiles())
                {
                    FileInfo targetFile = new(Path.Combine(toFolder, file.Name));
                    if (targetFile.Exists)
                    {
                        string bakPath = Path.Combine(bakDir.ToString(), targetFile.Name);
                        targetFile.MoveTo(bakPath, true);
                    }
                    file.MoveTo(Path.Combine(toFolder, file.Name));

                    readSize += (ulong)file.Length;
                    if (totalSize != 0) copyProgress.Report(readSize / totalSize);
                    
                }

                foreach (var dir in fromDir.GetDirectories())
                {
                    dir.MoveTo(Path.Combine(bakDir.ToString(), dir.Name));

                    readSize += dir.GetSize();
                    if (totalSize != 0) copyProgress.Report(readSize / totalSize);
                }

                result = true;
            }
            catch {}
            return result;
        }

        private static async Task<UpdaterResult> TryUnzipAsync(string path, IProgress<double> progress = null)
        {
            var result = UpdaterResult.ArchiveExtractionFailed;
            try
            {
                await ZipFileExtension.ExtractToDirectoryAsync(path, Path.GetDirectoryName(path), totalProg: progress, cancellationToken: m_cts.Token);
                result = UpdaterResult.Unpacked;
            }
            catch (Exception e)
            { 
                if (e is TaskCanceledException) result = UpdaterResult.Canceled;
            }
            return result;
        }

        private static async Task<(UpdaterResult Res, string Path)> TryDownloadAsync(string url, string fileName, IProgress<CurrentTotalBytes> progress = null)
        {
            var result = (Res: UpdaterResult.DownloadFailed, Path: string.Empty);
            string path = "";
            try
            {
                using var resp = await m_httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, m_cts.Token);
                resp.EnsureSuccessStatusCode();

                var contentLen = resp.Content.Headers.ContentLength;

                path = $"{AppDomain.CurrentDomain.BaseDirectory}{GenerateName()}";
                Directory.CreateDirectory(path);
                result.Path = $@"{path}\{fileName}";

                await using FileStream fs = File.Create(result.Path);
                await using var download = await resp.Content.ReadAsStreamAsync(m_cts.Token);
                if ((contentLen is null) || (progress is null))
                {
                    await download.CopyToAsync(fs, m_cts.Token);
                }
                else
                {
                    CurrentTotalBytes bytes = new((ulong)contentLen.Value);
                    var relProgress = new Progress<long>(readBytes =>
                    {
                        bytes.Current = (ulong)readBytes;
                        progress.Report(bytes);
                    });
                    await download.CopyToAsync(fs, relProgress, m_cts.Token);
                }
                result.Res = UpdaterResult.Downloaded;
            }
            catch (Exception e)
            {
                if (e is TaskCanceledException) result.Res = UpdaterResult.Canceled;

                result.Path = string.Empty;
                TryDeleteFolder(path);
            }
            return result;
        }

        private static string GenerateName()
        {
            Random rand = new();
            StringBuilder sbIn = new(Guid.NewGuid().ToString());
            StringBuilder sbOut = new();
            var len = sbIn.Length;
            for (int i = 0; i < len; i++)
            {
                // randomly remove '-' from guid to obfuscate name
                if (sbIn[i] == '-')
                {
                    if (rand.Next(100) < 60) continue;
                }
                sbOut.Append(sbIn[i]);
            }
            return sbOut.ToString();
        }

        private static bool TryDeleteFolder(string path)
        {
            try
            {
                if (!string.IsNullOrEmpty(path))
                {
                    Directory.Delete(path, true);
                }
                return true;
            }
            catch { }
            return false;
        }

        private static bool IsArchitectureMatch(string name)
        {
            var arch = RuntimeInformation.ProcessArchitecture;
            switch (arch)
            {
                case Architecture.X86:
                case Architecture.X64:
                case Architecture.Arm:
                case Architecture.Arm64:
                    return name.Contains(arch.ToString(), StringComparison.OrdinalIgnoreCase);
                default:
                    return false;
            }
        }

        private static bool IsOsMatch(string name)
        {
            bool result = false;
            string strToCheck = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                strToCheck = "win";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                strToCheck = "linux";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                strToCheck = "macos";
            }
            return result = name.Contains(strToCheck, StringComparison.OrdinalIgnoreCase);
        }

        private static string FilterVersionString(string ver)
        {
            StringBuilder versionIn = new(ver);
            StringBuilder versionOut = new();
            var len = versionIn.Length;
            for (int i = 0; i < len; i++)
            {
                var ch = versionIn[i];
                if (char.IsDigit(ch) || (ch == '.'))
                {
                    versionOut.Append(ch);
                }
            }
            return versionOut.ToString();
        }
    }
}
