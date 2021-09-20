using System;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

namespace VoicemeeterOsdProgram.Core
{
    public static class ScreenWorkingAreaManager
    {
        private static Screen m_mainScreen;

        public static Screen MainScreen
        {
            get => IsMainScreenConnected() ? m_mainScreen : Screen.PrimaryScreen;
            set => m_mainScreen = value;
        }

        public static Rect GetWokringArea()
        {
            const int defMargin = 45;
            const int defHeight = 1080;
            const int defWidth = 1920;
            const double defHorPercent = defMargin / defWidth;
            const double defVertPercent = defMargin / defHeight;

            var scr = MainScreen;
            var resolution = scr.Bounds;
            double marginH = (resolution.Width >= defWidth) ? defMargin : (double)(resolution.Width * defHorPercent);
            double marginV = (scr.Bounds.Height >= defHeight) ? defMargin : (double)(resolution.Height * defVertPercent);

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
    }
}
