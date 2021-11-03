using System;
using System.Runtime.InteropServices;
using System.Windows;
using static TopmostApp.Interop.NativeMethods;
using static VoicemeeterOsdProgram.Interop.NativeMethods;

namespace TopmostApp.Interop
{
    public partial class BandWindow
    {
        private void BandWindowExt()
        {
            Loaded += InitCustomProperties;
            Application.Current.Exit += OnAppExit;
        }

        public static readonly DependencyProperty LeftProperty = DependencyProperty.Register(
            "Left", typeof(double), typeof(BandWindow));
        public double Left
        {
            get => (double)GetValue(LeftProperty);
            set
            {
                SetPosition(value, ActualTop);
                SetValue(LeftProperty, value);
            }
        }

        public static readonly DependencyProperty TopProperty = DependencyProperty.Register(
            "Top", typeof(double), typeof(BandWindow));
        public double Top
        {
            get => (double)GetValue(TopProperty);
            set
            {
                SetPosition(ActualLeft, value);
                SetValue(TopProperty, value);
            }
        }

        public static readonly DependencyProperty IsClickThroughProperty = DependencyProperty.Register(
            "IsClickThrough", typeof(bool), typeof(BandWindow));
        public bool IsClickThrough
        {
            get => (bool)GetValue(IsClickThroughProperty);
            set
            {
                SetValue(IsClickThroughProperty, value);
                if (!IsLoaded) return;
                ToggleClickThrough(value);
            }
        }

        public static readonly DependencyProperty IsBgBlurredProperty = DependencyProperty.Register(
            nameof(IsBgBlurred), typeof(bool), typeof(BandWindow));
        public bool IsBgBlurred
        {
            get => (bool)GetValue(IsBgBlurredProperty);
            set
            {
                SetValue(IsBgBlurredProperty, value);
                if (!IsLoaded) return;
                TryToggleBgBlur(value);
            }
        }

        private bool TryToggleBgBlur(bool isEnabled)
        {
            bool result = false;
            try
            {
                var osVer = Environment.OSVersion.Version;
                var isWin10OrNewer = osVer >= new Version(10, 0);
                Version win10 = new(10, 0);
                Version win8 = new(6, 2);
                Version win7 = new(6, 1);
                if (osVer >= win10)
                {
                    ToggleBgBlurWin10(isEnabled);
                    result = true;
                }
                else if ((osVer >= win7) && (osVer < win8))
                {
                    ToggleBgBlurWin7(isEnabled);
                    result = true;
                }
            }
            catch { }
            return result;
        }

        private void ToggleBgBlurWin10(bool isEnabled)
        {
            AccentPolicy accent = new();
            accent.AccentState = isEnabled ? GetWin10BlurType() : AccentState.ACCENT_DISABLED;
            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            WindowCompositionAttributeData compData = new()
            {
                Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
                SizeOfData = accentStructSize,
                Data = accentPtr
            };

            SetWindowCompositionAttribute(Handle, ref compData);

            Marshal.FreeHGlobal(accentPtr);
        }

        private void ToggleBgBlurWin7(bool isEnabled)
        {
            DWM_BLURBEHIND bb = new(isEnabled);
            DwmEnableBlurBehindWindow(Handle, ref bb);
        }

        private static AccentState GetWin10BlurType()
        {
            /*var releaseId = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId", "").ToString();
            if (string.IsNullOrEmpty(releaseId))
            {
                releaseId = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows NT\CurrentVersion", "ReleaseId", "").ToString();
            }
            int.TryParse(releaseId, out int id);
            return id switch
            {
                >= 1809 => AccentState.ACCENT_ENABLE_HOSTBACKDROP,
                >= 1803 => AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND,
                _ => AccentState.ACCENT_ENABLE_BLURBEHIND
            };*/
            return AccentState.ACCENT_ENABLE_BLURBEHIND; // HOSTBACKDROP, ACRYLICBLURBEHIND are not working for some reason
        }

        private void ToggleClickThrough(bool isEnabled)
        {
            var hWnd = Handle;
            if (hWnd == IntPtr.Zero) return;
            int styles = GetWindowLongPtr(hWnd, (int)GetWindowLongFields.GWL_EXSTYLE).ToInt32();
            var stylesToApply = (int)ExtendedWindowStyles.WS_EX_LAYERED;
            if (isEnabled)
            {
                styles |= stylesToApply;
            }
            else
            {
                styles &= ~stylesToApply;
            }
            SetWindowLongPtr(hWnd, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)styles);
        }

        private void InitCustomProperties(object sender, RoutedEventArgs e)
        {
            if (IsClickThrough) ToggleClickThrough(true);
            if (IsBgBlurred) TryToggleBgBlur(true);
            SetPosition(Left, Top);
        }

        public void OnAppExit(object sender, EventArgs e)
        {
            // if program recieve termination signal and the window is shown "Invalid window handle" exception is thrown
            // same exception if trying to dispose hwndSource manually
            // so it's better to dispose it manually on Exit event, catch exception, so Dispose() will not be called automatically
            try
            {
                hwndSource?.Dispose();
            } catch { }
        }
    }
}
