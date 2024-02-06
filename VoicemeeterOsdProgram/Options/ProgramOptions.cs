using System;
using System.ComponentModel;
using System.Windows.Interop;

namespace VoicemeeterOsdProgram.Options;

public class ProgramOptions : OptionsBase
{
    private bool m_autostart = false;
    private RenderMode m_renderMode = RenderMode.Default;

    [Description(@"May appear as ""Application Frame Host"" in Startup tab of the Task Manager")]
    public bool Autostart
    {
        get => m_autostart;
        set => HandlePropertyChange(ref m_autostart, ref value, AutostartChanged);
    }

    [Description(@"Program uses hardware acceleration if posible by default. Use SoftwareOnly if your system have problems with GPU/drivers")]
    public RenderMode RenderMode
    {
        get => m_renderMode;
        set => HandlePropertyChange(ref m_renderMode, ref value, RenderModeChanged);
    }

    public event EventHandler<bool> AutostartChanged;
    public event EventHandler<RenderMode> RenderModeChanged;
}
