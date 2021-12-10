using AtgDev.Utils.Extensions;
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

namespace VoicemeeterOsdProgram.Updater
{
    public static class UpdateManager
    {
        private const string Owner = "A-tG";
        private const string RepoName = "VoicemeeterFancyOSD";
        private const string ExtractedFolder = "VoicemeeterFancyOSD";

        private static Assembly m_assembly = Assembly.GetEntryAssembly();
        private static HttpClient m_httpClient = new();
        private static GitHubClient m_ghClient = new(new ProductHeaderValue(m_assembly.GetName().Name));
        private static ReleaseAsset m_latestAsset;
        private static CancellationTokenSource m_cancelToken = new();

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

        public static async Task<UpdaterResult> TryUpdateAsync(IProgress<CurrentTotalBytes> downloadProgress = null, IProgress<double> extractionProgress = null)
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
                if (TryRestartAppAndUpdateFiles(updateFolder))
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

        public static void CancelUpdate()
        {
            m_cancelToken.Cancel();
            m_cancelToken = new();
        }

        private static bool TryRestartAppAndUpdateFiles(string updateFolder)
        {
            try
            {
                string copyTo = Path.TrimEndingDirectorySeparator(AppDomain.CurrentDomain.BaseDirectory);
                string copyFrom = updateFolder + @$"\{ExtractedFolder}";
                string program = Environment.ProcessPath;

                // just in case, to avoid deleting wrong files
                bool isValidPaths = (Directory.GetParent(updateFolder).ToString() == copyTo) &&
                    (Directory.GetParent(program).ToString() == copyTo);
                if (string.IsNullOrEmpty(program) || !isValidPaths) return false;

                string programName = Path.GetFileName(program);

                string argument = "/C " +
                    $"taskkill /IM {programName} & " +
                    "timeout /t 2 /nobreak & " + // wait 2 seconds because in some cases Robocopy failes if it tries to copy right away 
                    $@"robocopy ""{copyFrom}"" ""{copyTo}"" /s /im /it /is /move & " +
                    $@"rmdir /Q /S ""{updateFolder}"" & " + // TO DO: find a way to delete old unused DLLs
                    $@"start """" /MIN ""{program}""";

                using Process p = new();
                p.StartInfo = new ProcessStartInfo()
                {
                    FileName = "cmd.exe",
                    Arguments = argument,
                    // Hides command promt completely
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                p.Start();

                return true;
            }
            catch { }
            return false;
        }

        private static async Task<UpdaterResult> TryUnzipAsync(string path, IProgress<double> progress = null)
        {
            var result = UpdaterResult.ArchiveExtractionFailed;
            try
            {
                await ZipFileExtensions.ExtractToDirectoryAsync(path, Path.GetDirectoryName(path), totalProg: progress, cancellationToken: m_cancelToken.Token);
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
                using var resp = await m_httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, m_cancelToken.Token);
                resp.EnsureSuccessStatusCode();

                var contentLen = resp.Content.Headers.ContentLength;

                path = $"{AppDomain.CurrentDomain.BaseDirectory}{GenerateName()}";
                Directory.CreateDirectory(path);
                result.Path = $@"{path}\{fileName}";

                await using FileStream fs = File.Create(result.Path);
                await using var download = await resp.Content.ReadAsStreamAsync(m_cancelToken.Token);
                if ((contentLen is null) || (progress is null))
                {
                    await download.CopyToAsync(fs, m_cancelToken.Token);
                }
                else
                {
                    CurrentTotalBytes bytes = new(contentLen.Value);
                    var relProgress = new Progress<long>(readBytes =>
                    {
                        bytes.Current = readBytes;
                        progress.Report(bytes);
                    });
                    await download.CopyToAsync(fs, relProgress, m_cancelToken.Token);
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
