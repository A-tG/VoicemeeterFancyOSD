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
            watcher.appsToDetect = new ListInFile(Globals.FullscreenAppsListFile)
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
        const string BaseExe = $"VoicemeeterFancyOsd.exe";
        string currentExe = $"{System.Diagnostics.Process.GetCurrentProcess().ProcessName}.exe";

        if (!AutostartManager.IsOsSupported)
        {
            return new();
        }

        AutostartManager autostart = new()
        {
            ProgramName = Program.Name,
            ProgramPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, currentExe),
            IconLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, BaseExe),
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
            var o = OptionsStorage.Logger;

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            Directory.CreateDirectory(path);
            logger = new(path)
            { 
                IsEnabled = o.Enabled,
                MaxLogFiles = o.LogFilesMax
            };
            o.EnabledChanged += (_, val) => logger.IsEnabled = val;
            o.LogFilesMaxChanged += (_, val) => logger.MaxLogFiles = val;
        }
        catch { }
        return logger;
    }
}
