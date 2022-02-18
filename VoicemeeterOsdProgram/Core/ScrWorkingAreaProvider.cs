using System.Windows;
using TopmostApp.Helpers;
using VoicemeeterOsdProgram.Options;
using WpfScreenHelper;

namespace VoicemeeterOsdProgram.Core
{
    public class ScrWorkingAreaProvider
    {
        public ScrWorkingAreaProvider(ScreenProvider scrProv)
        {
            ScreenProvider = scrProv;
        }

        public ScreenProvider ScreenProvider;

        public Rect GetWokringArea()
        {
            const double defMargin = 45;
            const double defHeight = 1080;
            const double defWidth = 1920;
            const double defHorPercent = defMargin / defWidth;
            const double defVertPercent = defMargin / defHeight;

            var scr = ScreenProvider?.MainScreen ?? Screen.PrimaryScreen;
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
