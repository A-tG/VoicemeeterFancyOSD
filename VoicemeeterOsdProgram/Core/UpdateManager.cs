using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AtgDev.Utils.Extensions;

namespace VoicemeeterOsdProgram.Core
{
    public static class UpdateManager
    {
        private const string Owner = "A-tG";
        private const string RepoName = "VoicemeeterFancyOSD";
        private const string ExtractedFolder = "VoicemeeterFancyOSD";

        private static Assembly m_assembly = Assembly.GetEntryAssembly();
        private static GitHubClient m_client;
        private static ReleaseAsset m_latestAsset;

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

        public static async Task<bool> TryCheckForUpdatesAsync()
        {
            bool result = false;

            try
            {
                var releases = await GetReleasesAsync();
                var rel = releases[0];

                var latestVer = new Version(FilterVersionString(rel.TagName));

                m_latestAsset = rel.Assets.First((el) => IsArchitectureMatch(el.Name));
                result = latestVer > CurrentVersion;

                LatestVersion = latestVer;
                LatestRelease = rel;
#if DEBUG // to be able download older version when debugging
                return true;
#endif
            }
            catch 
            {
                m_latestAsset = null;
            }

            return result;
        }

        public static async Task<bool> TryUpdate()
        {
            if (!await TryCheckForUpdatesAsync()) return false;

            var path = await TryDownloadAsync(m_latestAsset.BrowserDownloadUrl, m_latestAsset.Name);
            if (string.IsNullOrEmpty(path)) return false;

            if (await TryUnzip(path))
            {
                var updateFolder = Path.GetDirectoryName(path);
                if (TryRestartAppAndUpdateFiles(updateFolder))
                {
                    return true;
                }
                try
                {
                    // delete temprorary folder if update failed
                    Directory.Delete(updateFolder, true);
                }
                catch { }
            }
            return false;
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
                if (string.IsNullOrEmpty(program) && isValidPaths) return false;

                string programName = Path.GetFileName(program);

                string argument = "/C " +
                    $"taskkill /IM {programName} & " +
                    "timeout /t 2 /nobreak & " +
                    $@"robocopy ""{copyFrom}"" ""{copyTo}"" /s /im /it /is /move & " +
                    $@"rmdir /Q /S ""{updateFolder}"" & " + // TO DO: find a way to delete old unused DLLs
                    $@"start """" /MIN ""{program}""";

                using Process p = new();
                p.StartInfo = new ProcessStartInfo()
                {
                    FileName = "cmd.exe",
                    Arguments = argument,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                p.Start();
            }
            catch { }
            return true;
        }

        private static async Task<bool> TryUnzip(string path)
        {
            bool result = false;
            try
            {
                await ZipFileExntesions.ExtractToDirectoryAsync(path, Path.GetDirectoryName(path));
                result = true;
            }
            catch { }
            return result;
        }

        private static async Task<string> TryDownloadAsync(string url, string fileName)
        {
            var resultPath = string.Empty;
            try
            {
                using HttpClient client = new();
                var resp = await client.GetAsync(url);

                var path = @$"{AppDomain.CurrentDomain.BaseDirectory}{GenerateName()}";
                Directory.CreateDirectory(path);
                resultPath = $@"{path}\{fileName}";

                using FileStream fs = File.Create(resultPath);
                await resp.Content.CopyToAsync(fs);
            }
            catch 
            {
                resultPath = string.Empty;
            }
            return resultPath;
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
