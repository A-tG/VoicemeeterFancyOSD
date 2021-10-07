using System.Windows.Controls;
using System.Windows.Input;

namespace VoicemeeterOsdProgram.UiControls.OSD.Strip
{
    /// <summary>
    /// Interaction logic for FaderContainer.xaml
    /// </summary>
    public partial class FaderContainer : UserControl
    {
        public FaderContainer()
        {
            InitializeComponent();
        }

        private void OnFaderMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var slider = sender as Slider;
            if (sender is null) return;

            double val = 3;
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                val = 1;
            }
            else if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                val = 0.1;
            }
            if (e.Delta < 0) val *= -1;

            slider.Value += val;
        }

        private void OnFaderMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var slider = sender as Slider;
            if (sender is null) return;

            slider.Value = 0;
        }
    }
}
