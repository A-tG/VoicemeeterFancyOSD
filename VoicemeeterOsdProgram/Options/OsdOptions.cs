namespace VoicemeeterOsdProgram.Options
{
    public class OsdOptions
    {
        public bool IsShowOnlyIfVoicemeeterHidden { get; set; } = true;
        public bool IsInteractable { get; set; } = false;
        public int DurationMs { get; set; } = 2000;
    }
}
