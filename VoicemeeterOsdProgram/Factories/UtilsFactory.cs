using Atg.Utils;
using AtgDev.Utils;
using System;
using System.IO;
using TopmostApp.Helpers;
using VoicemeeterOsdProgram.Core;
using VoicemeeterOsdProgram.Options;

namespace VoicemeeterOsdProgram.Factories;

public static class UtilsFactory
{
    public static FullscreenAppsWatcher GetFullscreenAppsWatcher()
    {
        FullscreenAppsWatcher watcher = new();
        try
        {
            watcher.appsToDetect = new ListInFile(@$"{AppDomain.CurrentDomain.BaseDirectory}config\detect_apps.txt")
            {
                AllowDuplicates = false,
                IsCaseSensetive = false,
            };
        }
        catch { }
        var AltOsdOpt = OptionsStorage.AltOsdOptionsFullscreenApps;
        watcher.IsEnabled = AltOsdOpt.Enabled;
        AltOsdOpt.EnabledChanged += (_, val) => watcher.IsEnabled = val;
        return watcher;
    }

    public static AutostartManager GetAutostartManager()
    {
        const string ExeName = "VoicemeeterFancyOsd.exe";
        const string ExeHostName = "VoicemeeterFancyOsdHost.exe";

        var osVer = Environment.OSVersion.Version;
        var win8dot1Ver = new Version(6, 3);
        string exe = osVer < win8dot1Ver ? ExeName : ExeHostName;

        if (!AutostartManager.IsOsSupported)
        {
            return new AutostartManager();
        }

        AutostartManager autostart = new()
        {
            ProgramName = Program.Name,
            ProgramPath = AppDomain.CurrentDomain.BaseDirectory + exe,
            IconLocation = AppDomain.CurrentDomain.BaseDirectory + ExeName,
            IsEnabled = OptionsStorage.Program.Autostart,
        };
        OptionsStorage.Program.AutostartChanged += (_, val) => autostart.IsEnabled = val;
        return autostart;
    }

    public static ScreenProvider GetOsdScreenProvider()
    {
        ScreenProvider scrProv = new();
        scrProv.MainScreenIndex = Globals.Osd.fullscreenAppsWatcher.IsDetected ?
                OptionsStorage.AltOsdOptionsFullscreenApps.DisplayIndex :
                OptionsStorage.Osd.DisplayIndex;
        // potential memory leaks
        OptionsStorage.Osd.DisplayIndexChanged += (_, val) =>
        {
            if (Globals.Osd.fullscreenAppsWatcher.IsDetected) return;

            scrProv.MainScreenIndex = val;
        };
        OptionsStorage.AltOsdOptionsFullscreenApps.DisplayIndexChanged += (_, val) =>
        {
            if (!Globals.Osd.fullscreenAppsWatcher.IsDetected) return;

            scrProv.MainScreenIndex = val;
        };
        Globals.Osd.fullscreenAppsWatcher.IsDetectedChanged += (_, val) =>
        {
            scrProv.MainScreenIndex = val ? OptionsStorage.AltOsdOptionsFullscreenApps.DisplayIndex : OptionsStorage.Osd.DisplayIndex;
        };
        return scrProv;
    }

    public static Logger GetLogger()
    {
        Logger logger = null;
        try
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            Directory.CreateDirectory(path);
            logger = new(path)
            { 
                IsEnabled = true 
            };
        }
        catch { }
        return logger;
    }
}
