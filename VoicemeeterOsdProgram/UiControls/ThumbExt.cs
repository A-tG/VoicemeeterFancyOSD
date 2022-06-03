using System.Windows;
using System.Windows.Controls.Primitives;

namespace VoicemeeterOsdProgram.UiControls
{
    public class ThumbExt : Thumb
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value), typeof(double), typeof(ThumbExt));
        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }
}
