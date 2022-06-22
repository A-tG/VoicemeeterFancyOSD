using VoicemeeterOsdProgram.Types;

namespace VoicemeeterOsdProgram.UiControls.Settings
{
    public class AboutViewModel : BaseViewModel
    {
        public string Version
        {
            get => typeof(App).Assembly.GetName().Version.ToString();
            set { return; }
        }
    }
}
