using System;

namespace VoicemeeterOsdProgram.Options;

public class OtherOptions : OptionsBase
{
    private bool m_paused = false;

    public bool Paused
    {
        get => m_paused;
        set => HandlePropertyChange(ref m_paused, ref value, PausedChanged);
    }

    public event EventHandler<bool> PausedChanged;
}