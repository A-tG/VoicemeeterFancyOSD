using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoicemeeterOsdProgram.UiControls.Settings
{
    public class DisplayIndexInputOptionViewModel : InputOptionViewModel
    {
        public DisplayIndexInputOptionViewModel()
        {
            Label = "Display used index";
            TooltipText = "0 - is a primary display, 1 - is a secondary, etc";
        }
    }
}
