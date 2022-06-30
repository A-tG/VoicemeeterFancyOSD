using System.Windows.Controls.Primitives;

namespace VoicemeeterOsdProgram.UiControls
{
    public class PopupExt : Popup
    {
        public PopupExt()
        {
            MouseDown += (_, e) => e.Handled = true;
        }
    }
}
