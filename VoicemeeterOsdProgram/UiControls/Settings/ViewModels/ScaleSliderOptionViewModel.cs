namespace VoicemeeterOsdProgram.UiControls.Settings.ViewModels
{
    public class ScaleSliderOptionViewModel : SliderOptionViewModel
    {
        public ScaleSliderOptionViewModel()
        {
            SmallChange = 0.01;
            LargeChange = 0.05;
            TickFreq = 0.01;
            Min = 0.5;
            Max = 2;
        }
    }
}
