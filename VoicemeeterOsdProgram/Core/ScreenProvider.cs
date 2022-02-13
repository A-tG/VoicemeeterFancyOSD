using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            MainScreenIndex = OptionsStorage.Osd.DisplayIndex;
            OptionsStorage.Osd.DisplayIndexChanged += (_, val) => MainScreenIndex = val;
        }

        public static Screen MainScreen
        {
            get
            {
                var screens = Screen.AllScreens.ToArray();
                var len = screens.Length;
                return m_mainScreenIndex < len ? screens[m_mainScreenIndex] : Screen.PrimaryScreen;
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
                var len = screens.Length;
                if (value < len)
                {
                    m_mainScreenIndex = value;
                    MainScreen = screens[value];
                }
            }
        }

        public static Screen AltScreen
        {
            get
            {
                var screens = Screen.AllScreens.ToArray();
                var len = screens.Length;
                var index = OptionsStorage.AltOptionsForFullscreenApps.DisplayIndex;
                if (index < len)
                {
                    return screens[index];
                }
                else if (len > 1)
                {
                    return screens[^1];
                }
                return Screen.PrimaryScreen;
            }
        }

        public static EventHandler<Screen> MainScreenChanged;
    }
}
