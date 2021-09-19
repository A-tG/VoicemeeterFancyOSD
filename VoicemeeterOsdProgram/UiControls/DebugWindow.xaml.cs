using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VoicemeeterOsdProgram.Core;

namespace VoicemeeterOsdProgram.UiControls
{
    /// <summary>
    /// Interaction logic for DebugWindow.xaml
    /// </summary>
    public partial class DebugWindow : Window
    {
        public DebugWindow()
        {
            InitializeComponent();
        }

        private bool OnlyNumeric(string text)
        {
            Regex regex = new Regex("[^0-9.-]+");
            return !regex.IsMatch(text);
        }

        private void DurationInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = DurationInput.Text;
            if (uint.TryParse(text, out uint val))
            {
                OsdWindowManager.DurationMs = val;
            }
        }

        private void ScaleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            OsdWindowManager.Scale = e.NewValue;
        }

        private void ShowBtn_Click(object sender, RoutedEventArgs e)
        {
            OsdWindowManager.Show();
        }

        private void RandomizeBtn_Click(object sender, RoutedEventArgs e)
        {
            OsdWindowManager.RandomizeElementsState();
        }

        private void DurationInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(OnlyNumeric(e.Text));
            e.Handled = !OnlyNumeric(e.Text);
        }
    }
}
