using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using VoicemeeterOsdProgram.UiControls;
using VoicemeeterOsdProgram.UiControls.OSD.Strip;

namespace VoicemeeterOsdProgram.Factories
{
    public static class StripButtonFactory
    {

        public static ButtonContainer GetMono()
        {
            var btnCont = new ButtonContainer();
            var btn = btnCont.Btn;
            btnCont.Btn.Style = (Style)btnCont.Resources["MonoBtnStyle"];
            var icon = new Icon()
            {
                Style = (Style)btnCont.Resources["MonoIcon"],
                Margin = new Thickness(2)
            };
            btn.Icon = icon;
            return btnCont;
        }

        public static ButtonContainer GetMonoWithReverse()
        {
            var btnCont = new ButtonContainer();
            var btn = btnCont.Btn;
            btn.Style = (Style)btnCont.Resources["MonoReverseBtnStyle"];
            var icon = new Icon()
            {
                Style = (Style)btnCont.Resources["MonoReverseIcon"],
                Margin = new Thickness(2)
            };
            btn.Icon = icon;
            return btnCont;
        }

        public static ButtonContainer GetSolo()
        {
            var btnCont = new ButtonContainer();
            btnCont.Btn.Style = (Style)btnCont.Resources["SoloBtnStyle"];
            return btnCont;
        }

        public static ButtonContainer GetMute()
        {
            var btnCont = new ButtonContainer();
            var btn = btnCont.Btn;
            btn.Style = (Style)btnCont.Resources["MuteBtnStyle"];
            var icon = new Icon()
            {
                Style = (Style)btnCont.Resources["MuteIcon"],
                Margin = new Thickness(1)
            };
            btn.Icon = icon;
            return btnCont;
        }

        public static ButtonContainer GetBusSelect()
        {
            var btnCont = new ButtonContainer();
            return btnCont;
        }
    }
}
