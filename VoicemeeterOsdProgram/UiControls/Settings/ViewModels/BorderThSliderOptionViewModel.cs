namespace VoicemeeterOsdProgram.UiControls.Settings.ViewModels;

public class BorderThSliderOptionViewModel : SliderOptionViewModel
{
    public BorderThSliderOptionViewModel()
    {
        SmallChange = 0.01;
        LargeChange = 0.1;
        TickFreq = 0.01;
        Min = 0;
        Max = 4;
    }
}
