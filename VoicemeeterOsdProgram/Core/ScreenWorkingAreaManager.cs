using System;
using System.Linq;
using System.Windows;
using TopmostApp.Helpers;
using VoicemeeterOsdProgram.Options;
using WpfScreenHelper;

namespace VoicemeeterOsdProgram.Core
{
    public static class ScreenWorkingAreaManager
    {
        private static Screen m_mainScreen;
        private static uint m_mainScreenIndex;

        static ScreenWorkingAreaManager()
        {
            MainScreenIndex = OptionsStorage.Osd.DisplayIndex;
            OptionsStorage.Osd.DisplayIndexChanged += (_, val) => MainScreenIndex = val;
        }

        public static void Init() { }

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

        public static Rect GetWokringArea()
        {
            const double defMargin = 45;
            const double defHeight = 1080;
            const double defWidth = 1920;
            const double defHorPercent = defMargin / defWidth;
            const double defVertPercent = defMargin / defHeight;

            var isAltScreen = OptionsStorage.AltOptionsForFullscreenApps.Enabled;
            var scr = (isAltScreen && FullscreenAppsWatcher.IsDetected) ? AltScreen : MainScreen;
            var resolution = scr.Bounds;
            double marginH = (resolution.Width >= defWidth) ? defMargin : resolution.Width * defHorPercent;
            double marginV = (resolution.Height >= defHeight) ? defMargin : resolution.Height * defVertPercent;

            var wArea = scr.WorkingArea;
            wArea.Width -= marginH * 2;
            wArea.Height -= marginV * 2;
            wArea.X += marginH;
            wArea.Y += marginV;

            return wArea;
        }

        public static EventHandler<Screen> MainScreenChanged;
    }
}
