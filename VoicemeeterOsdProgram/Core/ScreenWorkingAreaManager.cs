using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public static System.Drawing.Rectangle GetWokringArea()
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

            var area = scr.WorkingArea;
            var x = (int)Math.Round(marginH);
            var y = (int)Math.Round(marginV);
            area.Width -= x * 2;
            area.Height -= y * 2;
            area.X += x;
            area.Y += y;

            return area;
        }

        private static bool IsMainScreenConnected()
        {
            if (m_mainScreen is null) return false;

            return Screen.AllScreens.Contains(m_mainScreen);
        }
    }
}
