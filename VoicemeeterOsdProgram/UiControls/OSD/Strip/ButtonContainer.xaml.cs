using System.Windows.Controls;

namespace VoicemeeterOsdProgram.UiControls.OSD.Strip
{
    /// <summary>
    /// Interaction logic for ButtonContainer.xaml
    /// </summary>
    public partial class ButtonContainer : ContentControl
    {
        public StripControl ParentStrip;

        public ButtonContainer()
        {
            InitializeComponent();
        }
    }
}
