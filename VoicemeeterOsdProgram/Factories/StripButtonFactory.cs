using System.Windows;
using VoicemeeterOsdProgram.Options;
using VoicemeeterOsdProgram.Types;
using VoicemeeterOsdProgram.UiControls;
using VoicemeeterOsdProgram.UiControls.OSD.Strip;

namespace VoicemeeterOsdProgram.Factories
{
    public static class StripButtonFactory
    {

        public static ButtonContainer GetMono(StripControl parent)
        {
            var btnCont = GetCommonBtnCont(parent);
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

        public static ButtonContainer GetMonoWithReverse(StripControl parent)
        {
            var btnCont = GetCommonBtnCont(parent);
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

        public static ButtonContainer GetSolo(StripControl parent)
        {
            var btnCont = GetCommonBtnCont(parent);
            btnCont.IsAlwaysVisible = () => OptionsStorage.Osd.AlwaysShowElements.Contains(StripElements.Solo);
            btnCont.Btn.Style = (Style)btnCont.Resources["SoloBtnStyle"];
            return btnCont;
        }

        public static ButtonContainer GetMute(StripControl parent)
        {
            var btnCont = GetCommonBtnCont(parent);
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

        public static ButtonContainer GetSel(StripControl parent)
        {
            var btnCont = GetCommonBtnCont(parent);
            var btn = btnCont.Btn;
            btn.Style = (Style)btnCont.Resources["SelBtnStyle"];
            return btnCont;
        }

        public static ButtonContainer GetBusSelect(StripControl parent, string name)
        {
            var btnCont = GetCommonBtnCont(parent);
            btnCont.Btn.Content = name;
            return btnCont;
        }

        public static ButtonContainer GetEqOn(StripControl parent)
        {
            var btnCont = GetCommonBtnCont(parent);
            btnCont.IsAlwaysVisible = () => OptionsStorage.Osd.AlwaysShowElements.Contains(StripElements.EQ);
            var btn = btnCont.Btn;
            btn.Content = "EQ";
            btn.Style = (Style)btnCont.Resources["EqOnBtnStyle"];
            return btnCont;
        }

        private static ButtonContainer GetCommonBtnCont(StripControl parent)
        {
            ButtonContainer btnCont = new();
            btnCont.ParentStrip = parent;
            return btnCont;
        }
    }
}
