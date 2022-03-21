using System.Windows.Controls;
using VoicemeeterOsdProgram.Types;

namespace VoicemeeterOsdProgram.UiControls.OSD.Strip
{
    /// <summary>
    /// Interaction logic for ButtonContainer.xaml
    /// </summary>
    public partial class ButtonContainer : ContentControl
    {
        public IOsdRootElement OsdParent;

        public ButtonContainer()
        {
            InitializeComponent();
        }
    }
}
