using System.Windows.Controls;
using VoicemeeterOsdProgram.Types;

namespace VoicemeeterOsdProgram.UiControls
{
    public class PopupButtonViewModel : BaseViewModel
    {
        private object m_btnContent;

        public object BtnContent
        {
            get => m_btnContent;
            set
            {
                m_btnContent = value;
                OnPropertyChanged();
            }
        }
    }
}
