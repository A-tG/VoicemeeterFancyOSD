using IWshRuntimeLibrary;
using System;
using System.IO;

namespace AtgDev.Utils;

public class AutostartManager
{
    private string m_shortcutPath;
    private bool m_isEnabled = false;

    public string ProgramName { get; init; }
    public string ProgramPath { get; init; }

    public string IconLocation { get; set; }

    public bool IsEnabled 
    { 
        get => m_isEnabled;
        set
        {
            if (m_isEnabled == value) return;

            TryToggle(value);
        }
    }

    static public bool IsOsSupported => OperatingSystem.IsWindows();

    public bool TryEnable() => TryToggle(true);

    public bool TryDisable() => TryToggle(false);

    public bool TryToggle(bool isEnabled)
    {
        try
        {
            Toggle(isEnabled);
            return true;
        }
        catch { }
        return false;
    }

    public void Enable() => Toggle(true);

    public void Disable() => Toggle(false);

    public void Toggle(bool isEnabled)
    {
        if (OperatingSystem.IsWindows())
        {
            WinToggle(isEnabled);
        }
        else
        {
            throw new PlatformNotSupportedException();
        }
        m_isEnabled = isEnabled;
    }

    private void WinToggle(bool isEnabled)
    {
        if (isEnabled)
        {
            WinEnable();
        }
        else
        {
            WinDisable();
        }
    }

    private void WinEnable()
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
    }

    private void WinDisable()
    {
        if (string.IsNullOrEmpty(m_shortcutPath)) return;

        System.IO.File.Delete(m_shortcutPath);
        m_shortcutPath = null;
    }
}
