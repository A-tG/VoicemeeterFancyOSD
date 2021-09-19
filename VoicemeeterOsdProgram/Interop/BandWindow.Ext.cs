using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static VoicemeeterOsdProgram.Interop.NativeMethods;

namespace VoicemeeterOsdProgram.Interop
{
    public partial class BandWindow
    {
        private void BandWindowExt()
        {
            Loaded += InitCustomProperties;
            App.Current.Exit += OnAppExit;
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
                styles &= stylesToApply;
            }
            SetWindowLongPtr(hWnd, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)styles);
        }
        private void InitCustomProperties(object sender, RoutedEventArgs e)
        {
            if (IsClickThrough) ToggleClickThrough(true);
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
