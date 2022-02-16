using Atg.Utils;
using System;
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

    public static ScreenProvider GetOsdScreenProvider()
    {
        ScreenProvider scrProv = new();
        scrProv.MainScreenIndex = OptionsStorage.AltOsdOptionsFullscreenApps.Enabled ?
                OptionsStorage.AltOsdOptionsFullscreenApps.DisplayIndex :
                OptionsStorage.Osd.DisplayIndex;
        OptionsStorage.Osd.DisplayIndexChanged += (_, val) =>
        {
            if (OptionsStorage.AltOsdOptionsFullscreenApps.Enabled) return;

            scrProv.MainScreenIndex = val;
        };
        OptionsStorage.AltOsdOptionsFullscreenApps.DisplayIndexChanged += (_, val) =>
        {
            if (!OptionsStorage.AltOsdOptionsFullscreenApps.Enabled) return;

            scrProv.MainScreenIndex = val;
        };
        Globals.fullscreenAppsWatcher.IsDetectedChanged += (_, val) =>
        {
            scrProv.MainScreenIndex = val ? OptionsStorage.AltOsdOptionsFullscreenApps.DisplayIndex : OptionsStorage.Osd.DisplayIndex;
        };
        return scrProv;
    }
}
