using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoicemeeterOsdProgram.Types;

namespace VoicemeeterOsdProgram.UiControls.Settings.ViewModels;

public class CheckboxWrapViewModel : BaseViewModel
{
    private object m_content, m_tooltipContent;

    public object Content
    {
        get => m_content;
        set
        {
            m_content = value;
            OnPropertyChanged();
        }
    }

    public object TooltipContent
    {
        get => m_tooltipContent;
        set
        {
            m_tooltipContent = value;
            OnPropertyChanged();
        }
    }
}
