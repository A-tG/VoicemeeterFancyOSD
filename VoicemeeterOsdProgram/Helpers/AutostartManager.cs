using IWshRuntimeLibrary;
using System;
using System.IO;

namespace AtgDev.Utils
{
    public class AutostartManager
    {
        public AutostartManager()
        {
            if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException();
        }

        public string ProgramName { get; set; }
        public string ProgramPath { get; set; }

        public string IconLocation { get; set; }

        public bool TrySetUp()
        {
            try
            {
                SetUp();
                return true;
            }
            catch { }
            return false;
        }

        private void SetUp()
        {
            const string AppdataVarName = "APPDATA";
            const string StartupPathTail = @"Microsoft\Windows\Start Menu\Programs\Startup";
            string AppdataRoam = Environment.GetEnvironmentVariable(AppdataVarName);
            if (string.IsNullOrEmpty(AppdataRoam))
            {
                throw new Exception($"{AppdataVarName} environment variable not found");
            }

            string startupPath = Path.Combine(AppdataRoam, StartupPathTail);
            if (!Directory.Exists(startupPath))
            {
                throw new DirectoryNotFoundException($"Startup folder not found: {startupPath}");
            }

            if (string.IsNullOrEmpty(ProgramName))
            {
                throw new ArgumentException($"{nameof(ProgramName)} need to be defined");
            }
            if (string.IsNullOrEmpty(ProgramPath))
            {
                throw new ArgumentException($"{nameof(ProgramPath)} need to be defined");
            }

            string shortcutPath = Path.Combine(startupPath, ProgramName + ".lnk");

            WshShellClass wsh = new();
            var shortcut = (IWshShortcut)wsh.CreateShortcut(shortcutPath);
            shortcut.TargetPath = ProgramPath;
            shortcut.WorkingDirectory = Path.GetDirectoryName(ProgramPath);
            shortcut.IconLocation = string.IsNullOrEmpty(IconLocation) ? ProgramPath : IconLocation;
            shortcut.Save();
        }
    }
}
