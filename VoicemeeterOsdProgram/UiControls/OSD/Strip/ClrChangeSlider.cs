using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VoicemeeterOsdProgram.UiControls.OSD.Strip
{
    public class ClrChangeSlider : Slider
    {
        public bool isIgnoreValueChanged;

        public ClrChangeSlider() : base()
        {
            ValueChanged += OnValueChange;
            Loaded += OnInit;
        }

        public static readonly DependencyProperty FirstBgProperty = DependencyProperty.Register(
            "FirstBg", typeof(Brush), typeof(ClrChangeSlider));
        public Brush FirstBg
        {
            get => (Brush)GetValue(FirstBgProperty);
            set => SetValue(FirstBgProperty, value);
        }

        public static readonly DependencyProperty SecondBgProperty = DependencyProperty.Register(
            "SecondBg", typeof(Brush), typeof(ClrChangeSlider));
        public Brush SecondBg
        {
            get => (Brush)GetValue(SecondBgProperty);
            set => SetValue(SecondBgProperty, value);
        }

        public static readonly DependencyProperty GreaterThanValChangeClrProperty = DependencyProperty.Register(
            "GreaterThanValChangeClr", typeof(int), typeof(ClrChangeSlider));
        public int GreaterThanValChangeClr
        {
            get => (int)GetValue(GreaterThanValChangeClrProperty);
            set => SetValue(GreaterThanValChangeClrProperty, value);
        }

        public bool IsChangeToSecondColor
        {
            get => m_isChangeToSecondColor;
            set
            {
                // "ValueChanged" can be triggered by adding "Value" attribute in the XAML before other properties are initialized
                if (!IsInitialized) return;
                if (m_isChangeToSecondColor != value)
                {
                    Background = value ? SecondBg : FirstBg;
                    m_isChangeToSecondColor = value;
                }
            }
        }

        private bool m_isChangeToSecondColor = false;

        private void OnValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateColor();
        }

        private void UpdateColor()
        {
            IsChangeToSecondColor = Value > GreaterThanValChangeClr;
        }

        private void OnInit(object sender, EventArgs e)
        {
            if (FirstBg is null) FirstBg = Background;
            UpdateColor();
        }
    }
}
