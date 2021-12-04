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
            }
            catch 
            {
                m_latestAsset = null;
            }

            return result;
        }

        public static async Task TryUpdate()
        {
            if (!await TryCheckForUpdatesAsync()) return;

            var path = await TryDownloadAsync(m_latestAsset.BrowserDownloadUrl, m_latestAsset.Name);
            if (string.IsNullOrEmpty(path)) return;

            if (await TryUnzip(path))
            {
                CopyAndUpdate(path);
                return;
            }

            try
            {
                // delete temprorary folder
                Directory.Delete(Path.GetDirectoryName(path), true);
            }
            catch { }
        }

        private static void CopyAndUpdate(string path)
        {
            string targerPath = Path.TrimEndingDirectorySeparator(AppDomain.CurrentDomain.BaseDirectory);
            string originPath = Path.GetDirectoryName(path) + @$"\{ExtractedFolder}";
            string program = Environment.ProcessPath;
            string programName = Path.GetFileName(program);

            string argument = "/C " +
                $"taskkill /IM {programName} & " +
                $@"robocopy ""{originPath}"" ""{targerPath}"" /s /im /it /is /move & " +
                $@"del /F /Q /S ""{Path.GetDirectoryName(path)}"" & " +
                $@"rmdir /Q /S ""{Path.GetDirectoryName(path)}"" & " +
                $@"start """" /MIN ""{program}""";

            try
            {
                using Process p = new();
                p.StartInfo = new ProcessStartInfo()
                {
                    FileName = "cmd.exe",
                    Arguments = argument
                };
                p.Start();
            }
            catch { }
        }

        private static async Task<bool> TryUnzip(string path)
        {
            bool result = false;
            try
            {
                await Task.Run(() => ZipFile.ExtractToDirectory(path, Path.GetDirectoryName(path)));
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

                var folderName = Guid.NewGuid().ToString().Replace("-", string.Empty);
                var path = @$"{AppDomain.CurrentDomain.BaseDirectory}{folderName}";
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
