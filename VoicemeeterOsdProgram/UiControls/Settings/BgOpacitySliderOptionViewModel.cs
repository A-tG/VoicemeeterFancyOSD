namespace VoicemeeterOsdProgram.UiControls.Settings
{
    public class BgOpacitySliderOptionViewModel : SliderOptionViewModel
    {
        public BgOpacitySliderOptionViewModel()
        {
            SmallChange = 0.01;
            LargeChange = 0.05;
            TickFreq = 0.01;
            Min = 0;
            Max = 1;
        }
    }
}
