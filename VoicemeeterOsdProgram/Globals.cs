using AtgDev.Utils;
using System;
using System.IO;
using TopmostApp.Helpers;
using VoicemeeterOsdProgram.Core;
using VoicemeeterOsdProgram.Factories;
using VoicemeeterOsdProgram.Options;

namespace VoicemeeterOsdProgram;

public static class Globals
{
    public static readonly AutostartManager autostartManager = UtilsFactory.GetAutostartManager();
    public static readonly Logger logger = UtilsFactory.GetLogger();
    public static readonly string FullscreenAppsListFile = Path.Combine(OptionsStorage.ConfigFolder, "detect_apps.txt");

    public static class Osd
    {
        public static readonly FullscreenAppsWatcher fullscreenAppsWatcher = UtilsFactory.GetFullscreenAppsWatcher();
        public static readonly ScreenProvider screenProvider = UtilsFactory.GetOsdScreenProvider();
        public static readonly ScrWorkingAreaProvider workingAreaProvider = new(screenProvider);
    }
}
