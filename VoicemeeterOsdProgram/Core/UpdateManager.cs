using Octokit;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace VoicemeeterOsdProgram.Core
{
    public static class UpdateManager
    {
        private const string Owner = "A-tG";
        private const string RepoName = "VoicemeeterFancyOSD";

        public static async Task<bool> TryCheckForUpdates()
        {
            bool result = false;

            try
            {
                var assembly = System.Reflection.Assembly.GetEntryAssembly();
                var programName = assembly.GetName().Name;

                GitHubClient client = new(new ProductHeaderValue(programName));
                var releases = await client.Repository.Release.GetAll(Owner, RepoName);

                var latestVer = new Version(FilterVersionString(releases[0].TagName));
                var programVer = assembly.GetName().Version;

                result = latestVer > programVer;
            }
            catch { }
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
