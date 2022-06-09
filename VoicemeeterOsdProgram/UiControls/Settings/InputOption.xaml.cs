using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VoicemeeterOsdProgram.UiControls.Settings
{
    /// <summary>
    /// Interaction logic for InputOption.xaml
    /// </summary>
    public partial class InputOption : UserControl
    {
        public Func<string, bool> filterTextFunc = (_) => true;

        public InputOption()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(InputValue), typeof(string), typeof(InputOption));
        public string InputValue
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private void PreviewText(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !filterTextFunc(e.Text);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(InputValue);
        }
    }
}
