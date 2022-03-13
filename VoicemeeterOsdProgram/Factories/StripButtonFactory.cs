using System.Windows;
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
            btnCont.Btn.Style = GetMonoStyle(btnCont);
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
            btn.Style = GetMonoWithReverseStyle(btnCont);
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
            btnCont.Btn.Style = GetSoloStyle(btnCont);
            return btnCont;
        }

        public static ButtonContainer GetMute()
        {
            var btnCont = new ButtonContainer();
            var btn = btnCont.Btn;
            btn.Style = GetMuteStyle(btnCont);
            var icon = new Icon()
            {
                Style = (Style)btnCont.Resources["MuteIcon"],
                Margin = new Thickness(1)
            };
            btn.Icon = icon;
            return btnCont;
        }

        public static ButtonContainer GetSel()
        {
            var btnCont = new ButtonContainer();
            var btn = btnCont.Btn;
            btn.Style = GetSelStyle(btnCont);
            return btnCont;
        }

        public static Style GetMonoStyle(ButtonContainer btnCont) => (Style)btnCont.Resources["MonoBtnStyle"];

        public static Style GetMonoWithReverseStyle(ButtonContainer btnCont) => (Style)btnCont.Resources["MonoReverseBtnStyle"];

        public static Style GetSoloStyle(ButtonContainer btnCont) => (Style)btnCont.Resources["SoloBtnStyle"];

        public static Style GetMuteStyle(ButtonContainer btnCont) => (Style)btnCont.Resources["MuteBtnStyle"];

        public static Style GetSelStyle(ButtonContainer btnCont) => (Style)btnCont.Resources["SelBtnStyle"];

        public static ButtonContainer GetBusSelect()
        {
            var btnCont = new ButtonContainer();
            return btnCont;
        }
    }
}
