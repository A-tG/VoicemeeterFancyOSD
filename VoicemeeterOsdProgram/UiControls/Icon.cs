using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VoicemeeterOsdProgram.UiControls
{
    public class Icon : Control
    {
        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(
            "Stretch", typeof(Stretch), typeof(Icon));
        public Stretch Stretch
        {
            get => (Stretch)GetValue(StretchProperty);
            set => SetValue(StretchProperty, value);
        }

        public static readonly DependencyProperty NormalColorProperty = DependencyProperty.Register(
            "NormalColor", typeof(Brush), typeof(Icon));
        public Brush NormalColor
        {
            get => (Brush)GetValue(NormalColorProperty);
            set => SetValue(NormalColorProperty, value);
        }

        public static readonly DependencyProperty ToggledColorProperty = DependencyProperty.Register(
            "ToggledColor", typeof(Brush), typeof(Icon));
        public Brush ToggledColor
        {
            get => (Brush)GetValue(ToggledColorProperty);
            set => SetValue(ToggledColorProperty, value);
        }

        public static readonly DependencyProperty StatesNumberProperty = DependencyProperty.Register(
            "StatesNumber", typeof(uint), typeof(Icon));
        public uint StatesNumber
        {
            get => (uint)GetValue(StatesNumberProperty);
            set => SetValue(StatesNumberProperty, value);
        }

        public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
            "State", typeof(uint), typeof(Icon));
        public uint State
        {
            get => (uint)GetValue(StateProperty);
            set
            {
                if (StatesNumber is 0) return;
                if (value >= StatesNumber)
                {
                    value = IsStateRotation ? value % StatesNumber : StatesNumber - 1;
                }
                SetValue(StateProperty, value);
            }
        }

        public static readonly DependencyProperty IsStateRotationProperty = DependencyProperty.Register(
            "IsStateRotation", typeof(bool), typeof(Icon));
        public bool IsStateRotation
        {
            get => (bool)GetValue(IsStateRotationProperty);
            set => SetValue(IsStateRotationProperty, value);
        }
    }
}
