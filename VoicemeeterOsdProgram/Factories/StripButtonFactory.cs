using System.Windows;
using VoicemeeterOsdProgram.Options;
using VoicemeeterOsdProgram.Types;
using VoicemeeterOsdProgram.UiControls;
using VoicemeeterOsdProgram.UiControls.OSD.Strip;

namespace VoicemeeterOsdProgram.Factories
{
    public static class StripButtonFactory
    {

        public static ButtonContainer GetMono()
        {
            var btnCont = new ButtonContainer();
            btnCont.IsAlwaysVisible = () => OptionsStorage.Osd.AlwaysShowElements.Contains(StripElements.Mono);

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
            btnCont.IsAlwaysVisible = () => OptionsStorage.Osd.AlwaysShowElements.Contains(StripElements.Mono);

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
            btnCont.IsAlwaysVisible = () => OptionsStorage.Osd.AlwaysShowElements.Contains(StripElements.Solo);
            btnCont.Btn.Style = (Style)btnCont.Resources["SoloBtnStyle"];
            return btnCont;
        }

        public static ButtonContainer GetMute()
        {
            var btnCont = new ButtonContainer();
            btnCont.IsAlwaysVisible = () => OptionsStorage.Osd.AlwaysShowElements.Contains(StripElements.Mute);

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

        public static ButtonContainer GetSel()
        {
            var btnCont = new ButtonContainer();
            var btn = btnCont.Btn;
            btn.Style = (Style)btnCont.Resources["SelBtnStyle"];
            return btnCont;
        }

        public static ButtonContainer GetBusSelect()
        {
            var btnCont = new ButtonContainer();
            return btnCont;
        }

        public static ButtonContainer GetEqOn()
        {
            var btnCont = new ButtonContainer();
            btnCont.IsAlwaysVisible = () => OptionsStorage.Osd.AlwaysShowElements.Contains(StripElements.EQ);
            var btn = btnCont.Btn;
            btn.Content = "EQ";
            btn.Style = (Style)btnCont.Resources["EqOnBtnStyle"];
            return btnCont;
        }
    }
}
