using System;
using System.Linq;
using TopmostApp.Helpers;
using VoicemeeterOsdProgram.Options;
using WpfScreenHelper;

namespace VoicemeeterOsdProgram.Core
{
    public static class ScreenProvider
    {
        private static Screen m_mainScreen;
        private static uint m_mainScreenIndex;

        static ScreenProvider()
        {
            MainScreenIndex = OptionsStorage.AltOsdOptionsFullscreenApps.Enabled ?
                OptionsStorage.AltOsdOptionsFullscreenApps.DisplayIndex : 
                OptionsStorage.Osd.DisplayIndex;
            OptionsStorage.Osd.DisplayIndexChanged += (_, val) =>
            {
                if (OptionsStorage.AltOsdOptionsFullscreenApps.Enabled) return;

                MainScreenIndex = val;
            };
            OptionsStorage.AltOsdOptionsFullscreenApps.DisplayIndexChanged += (_, val) =>
            {
                if (!OptionsStorage.AltOsdOptionsFullscreenApps.Enabled) return;

                MainScreenIndex = val;
            };
            FullscreenAppsWatcher.IsDetectedChanged += (_, val) =>
            {
                MainScreenIndex = val ? OptionsStorage.AltOsdOptionsFullscreenApps.DisplayIndex : OptionsStorage.Osd.DisplayIndex;
            };
        }

        public static Screen MainScreen
        {
            get
            {
                var screens = Screen.AllScreens.ToArray();
                return m_mainScreenIndex < screens.Length ? screens[m_mainScreenIndex] : Screen.PrimaryScreen;
            }
            private set
            {
                if (m_mainScreen == value) return;

                m_mainScreen = value;
                MainScreenChanged?.Invoke(null, value);
            }
        }

        public static uint MainScreenIndex
        {
            get => m_mainScreenIndex;
            set
            {
                var screens = Screen.AllScreens.ToArray();
                if (value < screens.Length)
                {
                    m_mainScreenIndex = value;
                    MainScreen = screens[value];
                }
            }
        }

        public static EventHandler<Screen> MainScreenChanged;
    }
}
