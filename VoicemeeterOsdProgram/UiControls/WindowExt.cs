using System;
using System.Windows;
using System.Windows.Interop;
using TopmostApp.Interop;
using static TopmostApp.Interop.NativeMethods;


namespace VoicemeeterOsdProgram.UiControls;

public class WindowExt : Window
{
    public WindowExt() : base()
    {
        Loaded += OnLoad;
    }

    public static readonly DependencyProperty IsClickThroughProperty = DependencyProperty.Register(
        "IsClickThrough", typeof(bool), typeof(WindowExt));
    public bool IsClickThrough
    {
        get => (bool)GetValue(IsClickThroughProperty);
        set
        {
            SetValue(IsClickThroughProperty, value);
            if (!IsLoaded) return;
            ToggleClickThrough(true);
        }
    }

    public static readonly DependencyProperty IsHiddenFromTasklistProperty = DependencyProperty.Register(
        "IsHiddenFromTasklist", typeof(bool), typeof(WindowExt));
    public bool IsHiddenFromTasklist
    {
        get => (bool)GetValue(IsHiddenFromTasklistProperty);
        set
        {
            SetValue(IsHiddenFromTasklistProperty, value);
            if (!IsLoaded) return;
            ToggleHideFromTasklist(value);
        }
    }

    public IntPtr Hwnd => new WindowInteropHelper(this).Handle;

    private void OnLoad(object sender, RoutedEventArgs e)
    {
        if (IsClickThrough) ToggleClickThrough(true);
        if (IsHiddenFromTasklist) ToggleHideFromTasklist(true);
    }

    private void ToggleClickThrough(bool isEnable)
    {
        var hWnd = Hwnd;
        if (hWnd == IntPtr.Zero) return;
        int styles = GetWindowLongPtr(hWnd, (int)GetWindowLongFields.GWL_EXSTYLE).ToInt32();
        if (isEnable)
        {
            if (AllowsTransparency == false)
            {
                throw new InvalidOperationException("AllowsTransparency have to be set True before window is shown");
            }
            styles |= (int)ExtendedWindowStyles.WS_EX_TRANSPARENT;
        }
        else
        {
            styles &= ~(int)ExtendedWindowStyles.WS_EX_TRANSPARENT;
        }
        SetWindowLongPtr(hWnd, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)styles);
    }

    private void ToggleHideFromTasklist(bool isEnable)
    {
        var hWnd = Hwnd;
        if (hWnd == IntPtr.Zero) return;
        int styles = GetWindowLongPtr(hWnd, (int)GetWindowLongFields.GWL_EXSTYLE).ToInt32();
        if (isEnable)
        {
            styles |= (int)ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            styles &= ~(int)ExtendedWindowStyles.WS_EX_APPWINDOW;
        }
        else
        {
            styles &= ~(int)ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            styles |= (int)ExtendedWindowStyles.WS_EX_APPWINDOW;
        }
        SetWindowLongPtr(hWnd, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)styles);
    }
}
