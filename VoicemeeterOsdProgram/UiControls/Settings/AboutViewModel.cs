using System.Runtime.InteropServices;
using System.Windows.Input;
using VoicemeeterOsdProgram.Types;
using VoicemeeterOsdProgram.UiControls.Helpers;

namespace VoicemeeterOsdProgram.UiControls.Settings
{
    public class AboutViewModel : BaseViewModel
    {
        public string Version
        {
            get => $"{RuntimeInformation.ProcessArchitecture} {typeof(App).Assembly.GetName().Version}";
            set { return; }
        }

        public ICommand OpenUriCommand { get; } = new OpenUrlCommand();
    }
}
