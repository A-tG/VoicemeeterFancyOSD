using VoicemeeterOsdProgram.Types;

namespace VoicemeeterOsdProgram.UiControls.Settings
{
    public class InputOptionViewModel: BaseViewModel
    {
        private string m_label = "";

        public string Label
        {
            get => m_label;
            set
            {
                m_label = value;
                OnPropertyChanged();
            }
        }
    }
}
