using IWshRuntimeLibrary;
using System;
using System.IO;

namespace AtgDev.Utils
{
    public class AutostartManager
    {
        private string m_shortcutPath;
        private bool m_isEnabled = false;

        public AutostartManager()
        {
            if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException();
        }

        public string ProgramName { get; init; }
        public string ProgramPath { get; init; }

        public string IconLocation { get; set; }

        public bool IsEnabled 
        { 
            get => m_isEnabled;
            set
            {
                if (m_isEnabled == value) return;

                _ = value ? TryEnable() : TryDisable();
            }
        }

        public bool TryEnable()
        {
            try
            {
                Enable();
                return true;
            }
            catch { }
            return false;
        }

        public bool TryDisable()
        {
            try
            {
                Disable();
                return true;
            }
            catch { }
            return false;
        }

        public void Enable()
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

            m_shortcutPath = shortcutPath;

            m_isEnabled = true;
        }

        public void Disable()
        {
            if (string.IsNullOrEmpty(m_shortcutPath)) return;

            System.IO.File.Delete(m_shortcutPath);

            m_isEnabled = false;
        }
    }
}
