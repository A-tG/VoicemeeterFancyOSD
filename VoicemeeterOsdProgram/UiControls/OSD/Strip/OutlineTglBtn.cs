using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace VoicemeeterOsdProgram.UiControls.OSD.Strip
{
    public class OutlineTglBtn : Button
    {
        public OutlineTglBtn() : base()
        {
            Click += OnClick;
        }

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius), typeof(CornerRadius), typeof(OutlineTglBtn));
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public static readonly DependencyProperty NormalColorProperty = DependencyProperty.Register(
            "NormalColor", typeof(Brush), typeof(OutlineTglBtn));
        public Brush NormalColor
        {
            get => (Brush)GetValue(NormalColorProperty);
            set => SetValue(NormalColorProperty, value);
        }

        public static readonly DependencyProperty ToggledColorProperty = DependencyProperty.Register(
            "ToggledColor", typeof(Brush), typeof(OutlineTglBtn));
        public Brush ToggledColor
        {
            get => (Brush)GetValue(ToggledColorProperty);
            set => SetValue(ToggledColorProperty, value);
        }

        public static readonly DependencyProperty StatesNumberProperty = DependencyProperty.Register(
            "StatesNumber", typeof(uint), typeof(OutlineTglBtn));
        public uint StatesNumber
        {
            get => (uint)GetValue(StatesNumberProperty);
            set => SetValue(StatesNumberProperty, value);
        }

        public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
            "State", typeof(uint), typeof(OutlineTglBtn));
        public uint State
        {
            get => (uint)GetValue(StateProperty);
            set
            {
                if ((StatesNumber is 0) || (State == value)) return;

                uint val = value < StatesNumber ? value : value % StatesNumber;
                SetValue(StateProperty, val);
            }
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon", typeof(Icon), typeof(OutlineTglBtn));
        public Icon Icon
        {
            get => (Icon)GetValue(IconProperty);
            set
            {
                var icon = value;
                if (Icon is not null)
                {
                    // remove bindings of old icon just for case
                    BindingOperations.ClearBinding(icon, Icon.NormalColorProperty);
                    BindingOperations.ClearBinding(icon, Icon.ToggledColorProperty);
                    BindingOperations.ClearBinding(icon, Icon.StateProperty);
                }
                var bind = new Binding("NormalColor") { Source = this };
                icon.SetBinding(Icon.NormalColorProperty, bind);
                bind = new Binding("ToggledColor") { Source = this };
                icon.SetBinding(Icon.ToggledColorProperty, bind);
                bind = new Binding("State") { Source = this };
                icon.SetBinding(Icon.StateProperty, bind);
                SetValue(IconProperty, value);
                Content = icon;
            }
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            if (!IsInitialized) return;
            State++;
        }
    }
}
