﻿using System.Windows;
using VoicemeeterOsdProgram.Types;
using VoicemeeterOsdProgram.UiControls;
using VoicemeeterOsdProgram.UiControls.OSD.Strip;

namespace VoicemeeterOsdProgram.Factories;

public static class StripButtonFactory
{

    public static ButtonContainer GetMono(IOsdRootElement parent)
    {
        var btnCont = GetCommonMono(parent);

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

    public static ButtonContainer GetMonoWithReverse(IOsdRootElement parent)
    {
        var btnCont = GetCommonMono(parent);

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

    public static ButtonContainer GetSolo(IOsdRootElement parent)
    {
        var btnCont = GetCommonBtnCont(parent);
        OsdContentFactory.InitChildElement(parent, btnCont, StripElements.Solo);
        btnCont.Btn.Style = (Style)btnCont.Resources["SoloBtnStyle"];
        return btnCont;
    }

    public static ButtonContainer GetMute(IOsdRootElement parent)
    {
        var btnCont = GetCommonBtnCont(parent);
        OsdContentFactory.InitChildElement(parent, btnCont, StripElements.Mute);

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

    public static ButtonContainer GetSel(IOsdRootElement parent)
    {
        var btnCont = GetCommonBtnCont(parent);
        var btn = btnCont.Btn;
        btn.Style = (Style)btnCont.Resources["SelBtnStyle"];
        return btnCont;
    }

    public static ButtonContainer GetBusSelect(IOsdRootElement parent, string name)
    {
        var btnCont = GetCommonBtnCont(parent);
        OsdContentFactory.InitChildElement(parent, btnCont, StripElements.Buses);
        btnCont.Btn.Content = name;
        return btnCont;
    }

    public static ButtonContainer GetEqOn(IOsdRootElement parent)
    {
        var btnCont = GetCommonBtnCont(parent);
        OsdContentFactory.InitChildElement(parent, btnCont, StripElements.EQ);
        var btn = btnCont.Btn;
        btn.Content = "EQ";
        btn.Style = (Style)btnCont.Resources["EqOnBtnStyle"];
        return btnCont;
    }

    private static ButtonContainer GetCommonBtnCont(IOsdRootElement parent)
    {
        ButtonContainer btnCont = new();
        btnCont.OsdParent = parent;
        return btnCont;
    }

    private static ButtonContainer GetCommonMono(IOsdRootElement parent)
    {
        var btnCont = GetCommonBtnCont(parent);
        OsdContentFactory.InitChildElement(parent, btnCont, StripElements.Mono);
        return btnCont;
    }
}
