using VoicemeeterOsdProgram.Types;

namespace VoicemeeterOsdProgram.UiControls.Settings.ViewModels
{
    public class OptionsSectionViewModel : BaseViewModel
    {
        private string m_label;

        public string LabelText
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
