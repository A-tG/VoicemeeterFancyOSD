using System;
using System.Windows.Controls;

namespace VoicemeeterOsdProgram.UiControls.OSD.Strip
{
    /// <summary>
    /// Interaction logic for ButtonContainer.xaml
    /// </summary>
    public partial class ButtonContainer : ContentControl
    {
        public Func<bool> IsAlwaysVisible = () => false;

        public ButtonContainer()
        {
            InitializeComponent();
        }
    }
}
