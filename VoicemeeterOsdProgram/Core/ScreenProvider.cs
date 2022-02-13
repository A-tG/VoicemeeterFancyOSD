using System;
using System.Linq;
using VoicemeeterOsdProgram.Options;
using WpfScreenHelper;

namespace VoicemeeterOsdProgram.Core
{
    public static class ScreenProvider
    {
        private static Screen m_mainScreen;
        private static Screen m_altScreen;
        private static uint m_mainScreenIndex;
        private static uint m_altScreenIndex;

        static ScreenProvider()
        {
            MainScreenIndex = OptionsStorage.Osd.DisplayIndex;
            AltScreenIndex = OptionsStorage.AltOptionsForFullscreenApps.DisplayIndex;
            OptionsStorage.Osd.DisplayIndexChanged += (_, val) => MainScreenIndex = val;
            OptionsStorage.AltOptionsForFullscreenApps.DisplayIndexChanged += (_, val) => AltScreenIndex = val;
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

        public static Screen AltScreen
        {
            get
            {
                var screens = Screen.AllScreens.ToArray();
                return m_altScreenIndex < screens.Length ? screens[m_altScreenIndex] : screens[^1];
            }
            private set
            {
                if (m_altScreen == value) return;

                m_altScreen = value;
                AltScreenChanged?.Invoke(null, value);
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

        public static uint AltScreenIndex
        {
            get => m_altScreenIndex;
            set
            {
                var screens = Screen.AllScreens.ToArray();
                if (value < screens.Length)
                {
                    m_altScreenIndex = value;
                    AltScreen = screens[value];
                }
            }
        }

        public static EventHandler<Screen> MainScreenChanged;
        public static EventHandler<Screen> AltScreenChanged;
    }
}
