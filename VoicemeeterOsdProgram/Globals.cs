using AtgDev.Utils;
using TopmostApp.Helpers;
using VoicemeeterOsdProgram.Core;
using VoicemeeterOsdProgram.Factories;

namespace VoicemeeterOsdProgram;

public static class Globals
{
    public static readonly AutostartManager autostartManager;
    public static readonly Logger logger = UtilsFactory.GetLogger();

    public static class Osd
    {
        public static readonly FullscreenAppsWatcher fullscreenAppsWatcher = UtilsFactory.GetFullscreenAppsWatcher();
        public static readonly ScreenProvider screenProvider = UtilsFactory.GetOsdScreenProvider();
        public static readonly ScrWorkingAreaProvider workingAreaProvider = new(screenProvider);
    }

    static Globals() 
    {
        autostartManager = UtilsFactory.GetAutostartManager();
    }

    static public void Init() { }
}
