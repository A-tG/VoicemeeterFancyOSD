namespace VoicemeeterOsdProgram.UiControls.Settings
{
    public class DisplayIndexInputOptionViewModel : InputOptionViewModel
    {
        public DisplayIndexInputOptionViewModel()
        {
            Label = "Display used index";
            TooltipText = "0 - is a primary display, 1 - is a secondary, etc";
        }
    }
}
