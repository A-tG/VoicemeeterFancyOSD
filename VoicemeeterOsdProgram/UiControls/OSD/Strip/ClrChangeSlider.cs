using System;
using System.Windows;
using System.Windows.Media;

namespace VoicemeeterOsdProgram.UiControls.OSD.Strip;

public class ClrChangeSlider : SliderExt
{
    private bool m_isChangeToSecondColor = false;

    public ClrChangeSlider() : base()
    {
        ValueChanged += OnValueChange;
        Loaded += OnInit;
    }

    public static readonly DependencyProperty FirstBgProperty = DependencyProperty.Register(
        nameof(FirstBg), typeof(Brush), typeof(ClrChangeSlider));
    public Brush FirstBg
    {
        get => (Brush)GetValue(FirstBgProperty);
        set => SetValue(FirstBgProperty, value);
    }

    public static readonly DependencyProperty SecondBgProperty = DependencyProperty.Register(
        nameof(SecondBg), typeof(Brush), typeof(ClrChangeSlider));
    public Brush SecondBg
    {
        get => (Brush)GetValue(SecondBgProperty);
        set => SetValue(SecondBgProperty, value);
    }

    public static readonly DependencyProperty FirstFgProperty = DependencyProperty.Register(
        nameof(FirstFg), typeof(Brush), typeof(ClrChangeSlider));
    public Brush FirstFg
    {
        get => (Brush)GetValue(FirstFgProperty);
        set => SetValue(FirstFgProperty, value);
    }

    public static readonly DependencyProperty SecondFgProperty = DependencyProperty.Register(
        nameof(SecondFg), typeof(Brush), typeof(ClrChangeSlider));
    public Brush SecondFg
    {
        get => (Brush)GetValue(SecondFgProperty);
        set => SetValue(SecondFgProperty, value);
    }

    public static readonly DependencyProperty GreaterThanValChangeClrProperty = DependencyProperty.Register(
        nameof(GreaterThanValChangeClr), typeof(double?), typeof(ClrChangeSlider));
    public double? GreaterThanValChangeClr
    {
        get => (double?)GetValue(GreaterThanValChangeClrProperty);
        set => SetValue(GreaterThanValChangeClrProperty, value);
    }

    public static readonly DependencyProperty GreaterOrEqualValChangeClrProperty = DependencyProperty.Register(
        nameof(GreaterOrEqualValChangeClr), typeof(double?), typeof(ClrChangeSlider));
    public double? GreaterOrEqualValChangeClr
    {
        get => (double?)GetValue(GreaterOrEqualValChangeClrProperty);
        set => SetValue(GreaterOrEqualValChangeClrProperty, value);
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
                Foreground = value ? SecondFg : FirstFg;
                m_isChangeToSecondColor = value;
            }
        }
    }

    private void OnValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        UpdateColor();
    }

    private void UpdateColor()
    {
        // reading Value before Loaded event can break Value displaying on element
        if (!IsLoaded) return;

        if (GreaterOrEqualValChangeClr is null)
        {
            IsChangeToSecondColor = Value > GreaterThanValChangeClr;
        }
        else if (GreaterThanValChangeClr is null)
        {
            IsChangeToSecondColor = Value >= GreaterOrEqualValChangeClr;
        }
    }

    private void OnInit(object sender, EventArgs e)
    {
        if (FirstBg is null) FirstBg = Background;
        if (SecondBg is null) SecondBg = Background;
        if (FirstFg is null) FirstFg = Foreground;
        if (SecondFg is null) SecondFg = Foreground;
        UpdateColor();
    }
}
