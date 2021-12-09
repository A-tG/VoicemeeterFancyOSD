using AtgDev.Utils.Extensions;
using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoicemeeterOsdProgram.Updater.Types;

namespace VoicemeeterOsdProgram.Updater
{
    public static class UpdateManager
    {
        private const string Owner = "A-tG";
        private const string RepoName = "VoicemeeterFancyOSD";
        private const string ExtractedFolder = "VoicemeeterFancyOSD";

        private static Assembly m_assembly = Assembly.GetEntryAssembly();
        private static GitHubClient m_client;
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

        private static GitHubClient Client
        {
            get
            {
                if (m_client is null)
                {
                    m_client = new(new ProductHeaderValue(m_assembly.GetName().Name));
                }
                return m_client;
            }
        }

        public static async Task<UpdaterResult> TryCheckForUpdatesAsync()
        {
            UpdaterResult result = default;

            try
            {
                var releases = await GetReleasesAsync();
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
            catch (ApiException)
            {
                result = UpdaterResult.ConnectionError;
            }
            catch (HttpRequestException)
            {
                result = UpdaterResult.ConnectionError;
            }
            catch (InvalidOperationException)
            {
                result = UpdaterResult.ArchitectureNotFound;
            }
            catch
            {
                m_latestAsset = null;
            }

            return result;
        }

        public static async Task<UpdaterResult> TryUpdate(IProgress<double> progress = null)
        {
            try
            {
                var result = await TryCheckForUpdatesAsync();
                if (result != UpdaterResult.NewVersionFound) return result;

                var downloadRes = await TryDownloadAsync(m_latestAsset.BrowserDownloadUrl, m_latestAsset.Name, progress);
                if (!downloadRes.IsSuccess || string.IsNullOrEmpty(downloadRes.Path)) return UpdaterResult.DownloadFailed;

                var path = downloadRes.Path;
                var updateFolder = Path.GetDirectoryName(path);
                if (await TryUnzip(path, progress))
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
                    return UpdaterResult.ArchiveExtractionFailed;
                }
            }
            catch (TaskCanceledException)
            {
                return UpdaterResult.Canceled;
            }
        }

        public static void CancelUpdate()
        {
            m_cancelToken.Cancel();
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

        private static async Task<bool> TryUnzip(string path, IProgress<double> progress = null)
        {
            bool result = false;
            try
            {
                await ZipFileExtensions.ExtractToDirectoryAsync(path, Path.GetDirectoryName(path), totalProg: progress, cancellationToken: m_cancelToken.Token);
                result = true;
            }
            catch { }
            return result;
        }

        private static async Task<(bool IsSuccess, string Path)> TryDownloadAsync(string url, string fileName, IProgress<double> progress = null)
        {
            var result = (IsSuccess: false, Path: string.Empty);
            try
            {
                using HttpClient client = new();
                var resp = await client.GetAsync(url, m_cancelToken.Token);
                var contentLen = resp.Content.Headers.ContentLength;

                var path = $"{AppDomain.CurrentDomain.BaseDirectory}{GenerateName()}";
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
                    var len = contentLen.Value;
                    var relProgress = new Progress<double>(total => progress.Report(total * 100.0 / len));
                    // PROBLEM: doesn't change UI values for some reason
                    await download.CopyToAsync(fs, relProgress, m_cancelToken.Token);
                }
                result.IsSuccess = true;
            }
            catch 
            {
                result.Path = string.Empty;
            }
            return result;
        }

        private static async Task<IReadOnlyList<Release>> GetReleasesAsync()
        {
            return await Client.Repository.Release.GetAll(Owner, RepoName);
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
                Directory.Delete(path, true);
                return true;
            }
            catch { }
            return false;
        }

        private static bool IsArchitectureMatch(string name)
        {
            bool result = false;
            if (Environment.Is64BitProcess)
            {
                result = name.Contains("x64");
            }
            else
            {
                result = name.Contains("x86");
            }
            return result;
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
