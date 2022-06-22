using System.Windows.Input;
using VoicemeeterOsdProgram.Types;
using VoicemeeterOsdProgram.UiControls.Helpers;

namespace VoicemeeterOsdProgram.UiControls.Settings
{
    public class AboutViewModel : BaseViewModel
    {
        public string Version
        {
            get => typeof(App).Assembly.GetName().Version.ToString();
            set { return; }
        }

        public ICommand OpenUriCommand { get; } = new OpenUrlCommand();
    }
}
