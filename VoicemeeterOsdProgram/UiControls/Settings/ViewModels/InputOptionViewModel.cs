using VoicemeeterOsdProgram.Types;

namespace VoicemeeterOsdProgram.UiControls.Settings.ViewModels;

public class InputOptionViewModel: BaseViewModel
{
    private string m_label = "", m_tooltipText = "";

    public string Label
    {
        get => m_label;
        set
        {
            m_label = value;
            OnPropertyChanged();
        }
    }

    public string TooltipText
    {
        get => m_tooltipText;
        set
        {
            m_tooltipText = value;
            OnPropertyChanged();
        }
    }
}
