namespace VoicemeeterOsdProgram.UiControls.Settings.ViewModels;

public class DisplayIndexInputOptionViewModel : InputOptionViewModel
{
    public DisplayIndexInputOptionViewModel()
    {
        Label = "Display";
        TooltipText = "The index of the display being used.\n0 - is a primary display, 1 - is a secondary, etc";
    }
}
