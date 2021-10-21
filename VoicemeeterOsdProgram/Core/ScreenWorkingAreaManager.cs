using System;
using System.Linq;
using System.Windows;
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
            get => IsMainScreenConnected() ? m_mainScreen : Screen.PrimaryScreen;
            set
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

        public static Rect GetWokringArea()
        {
            const double defMargin = 45;
            const double defHeight = 1080;
            const double defWidth = 1920;
            const double defHorPercent = defMargin / defWidth;
            const double defVertPercent = defMargin / defHeight;

            var scr = MainScreen;
            var resolution = scr.Bounds;
            double marginH = (resolution.Width >= defWidth) ? defMargin : resolution.Width * defHorPercent;
            double marginV = (scr.Bounds.Height >= defHeight) ? defMargin : resolution.Height * defVertPercent;

            var wArea = scr.WorkingArea;
            Rect area = new()
            { 
                X = wArea.X, 
                Y = wArea.Y,
                Width = wArea.Width,
                Height = wArea.Height
            };
            area.Width -= marginH * 2;
            area.Height -= marginV * 2;
            area.X += marginH;
            area.Y += marginV;

            return area;
        }

        private static bool IsMainScreenConnected()
        {
            if (m_mainScreen is null) return false;

            return Screen.AllScreens.Contains(m_mainScreen);
        }

        public static EventHandler<Screen> MainScreenChanged;
    }
}
