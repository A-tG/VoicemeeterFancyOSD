using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TopmostApp.Helpers;
using VoicemeeterOsdProgram.Options;

namespace VoicemeeterOsdProgram.Core
{
    public static class ScrWorkingAreaProvider
    {
        public static Rect GetWokringArea()
        {
            const double defMargin = 45;
            const double defHeight = 1080;
            const double defWidth = 1920;
            const double defHorPercent = defMargin / defWidth;
            const double defVertPercent = defMargin / defHeight;

            var isAltScreen = OptionsStorage.AltOptionsForFullscreenApps.Enabled;
            var scr = (isAltScreen && FullscreenAppsWatcher.IsDetected) ? ScreenProvider.AltScreen : ScreenProvider.MainScreen;
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
    }
}
