using System.Reflection;
using VoicemeeterOsdProgram.Types;

namespace VoicemeeterOsdProgram.UiControls.Settings
{
    public class AboutViewModel : BaseViewModel
    {
        public string Version
        {
            get => Assembly.GetEntryAssembly().GetName().Version.ToString();
            set { return; }
        }
    }
}
