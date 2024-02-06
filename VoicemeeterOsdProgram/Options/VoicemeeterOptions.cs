using System;
using System.ComponentModel;
using VoicemeeterOsdProgram.Core;

namespace VoicemeeterOsdProgram.Options;

public class VoicemeeterOptions : OptionsBase
{
    private VoicemeeterApiClient.Rate m_apiPollingRate = VoicemeeterApiClient.Rate.Normal;
    private uint m_initDelay = 0;

    [Description("How fast Voicemeeter API is called to to track changes. Recommended: Normal or Fast. Faster rate - more cpu usage and OSD is more responsive to quick changes. Options represent 15, 30, 60, 144hz")]
    public VoicemeeterApiClient.Rate ApiPollingRate
    {
        get => m_apiPollingRate;
        set => HandlePropertyChange(ref m_apiPollingRate, ref value, ApiPollingRateChanged);
    }

    [Description("Wait ms before loading/connecting to Voicemeeter Remote API. Might help if program experience problems with vmrapi on system startup")]
    public uint InitializationDelay
    {
        get => m_initDelay;
        set
        {
            value = value > int.MaxValue ? int.MaxValue : value;
            HandlePropertyChange(ref m_initDelay, ref value, DelayInitializationChanged);
        }
    }

    public event EventHandler<VoicemeeterApiClient.Rate> ApiPollingRateChanged;
    public event EventHandler<uint> DelayInitializationChanged;
}
