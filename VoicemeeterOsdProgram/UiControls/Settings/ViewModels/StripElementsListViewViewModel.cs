using System.Collections.Generic;
using VoicemeeterOsdProgram.Types;
using VoicemeeterOsdProgram.UiControls.ViewModels;

namespace VoicemeeterOsdProgram.UiControls.Settings.ViewModels;

public class StripElementsListViewViewModel : PopupButtonViewModel
{
    public Dictionary<StripElements, string> PossibleValues { get; } = new()
    {
        { StripElements.Mute, "Mute Button" },
        { StripElements.Mono, "Mono Button" },
        { StripElements.Solo, "Solo Button" },
        { StripElements.EQ, "EQ Button" },
        { StripElements.Buses, "Buses select" },
        { StripElements.Fader, "Fader (gain slider)" },
        { StripElements.Limiter, "Limiter slider" }
    };
}
