using System.Collections.Generic;
using System.Windows.Controls;
using VoicemeeterOsdProgram.Core;
using VoicemeeterOsdProgram.Options;

namespace VoicemeeterOsdProgram.UiControls.Settings;

/// <summary>
/// Interaction logic for Voicemeeter.xaml
/// </summary>
public partial class Voicemeeter : UserControl
{
    public Dictionary<VoicemeeterApiClient.Rate, string> RateValues { get; } = new()
    {
        { VoicemeeterApiClient.Rate.Slow, "Slow (15hz)" },
        { VoicemeeterApiClient.Rate.Normal, "Normal (30hz)" },
        { VoicemeeterApiClient.Rate.Fast, "Fast (60hz)" },
        { VoicemeeterApiClient.Rate.VeryFast, "Very Fast (140hz)" },
    };

    public Voicemeeter()
    {
        DataContext = OptionsStorage.Voicemeeter;
        InitializeComponent();
    }
}
