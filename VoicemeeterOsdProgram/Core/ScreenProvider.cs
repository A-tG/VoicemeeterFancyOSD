using System;
using System.Linq;
using TopmostApp.Helpers;
using VoicemeeterOsdProgram.Options;
using WpfScreenHelper;

namespace VoicemeeterOsdProgram.Core
{
    public class ScreenProvider
    {
        private Screen m_mainScreen;
        private uint m_mainScreenIndex;

        public Screen MainScreen
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

        public uint MainScreenIndex
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

        public EventHandler<Screen> MainScreenChanged;
    }
}
