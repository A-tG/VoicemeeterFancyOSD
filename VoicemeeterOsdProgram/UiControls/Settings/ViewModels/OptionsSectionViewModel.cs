using VoicemeeterOsdProgram.Types;

namespace VoicemeeterOsdProgram.UiControls.Settings.ViewModels
{
    public class OptionsSectionViewModel : BaseViewModel
    {
        private string m_label;
        private object m_tooltip;

        public string LabelText
        {
            get => m_label;
            set
            {
                m_label = value;
                OnPropertyChanged();
            }
        }

        public object TooltipContent
        {
            get => m_tooltip;
            set
            {
                m_tooltip = value;
                OnPropertyChanged();
            }
        }
    }
}
