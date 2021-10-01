using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using VoicemeeterOsdProgram.Core.Types;
using VoicemeeterOsdProgram.Factories;
using VoicemeeterOsdProgram.Types;
using VoicemeeterOsdProgram.UiControls.OSD;
using VoicemeeterOsdProgram.UiControls.OSD.Strip;
using AtgDev.Voicemeeter.Types;
using static TopmostApp.Interop.NativeMethods;

namespace VoicemeeterOsdProgram.Core
{
    public static partial class OsdWindowManager
    {
        private static OsdControl m_wpfControl;
        private static OsdWindow m_window;
        private static DispatcherTimer m_TickTimer;
        private static VoicemeeterParameter[] m_vmParams;

        static OsdWindowManager()
        {
            OsdControl osd = new();
            osd.Background.Opacity = 0.9;
            m_wpfControl = osd;
            ApplyVisibilityToOsdElements(Visibility.Collapsed);

            var win = new OsdWindow()
            {
                WorkingAreaVertAlignment = VertAlignment.Top,
                WorkingAreaHorAlignment = HorAlignment.Right,
                Content = osd,
                Activatable = false,
                TopMost = true,
                IsClickThrough = true,
                ZBandID = GetTopMostZBandID()
            };
            win.CreateWindow();
            m_window = win;

            m_TickTimer = new(DispatcherPriority.Normal);
            m_TickTimer.Interval = TimeSpan.FromMilliseconds(2000);
            m_TickTimer.Tick += TimerTick;

            VoicemeeterApiClient.NewParameters += OnNewVoicemeeterParams;
            VoicemeeterApiClient.ProgramTypeChange += OnVoicemeeterTypeChange;
        }

        public static void Init() { }

        public static bool IsEnabled
        {
            get => VoicemeeterApiClient.IsHandlingParams;
            set => VoicemeeterApiClient.IsHandlingParams = value;
        }
        public static double Scale
        {
            get => m_wpfControl.Scale;
            set => m_wpfControl.Scale = value;
        }

        public static double DurationMs
        {
            get => m_TickTimer.Interval.TotalMilliseconds;
            set => m_TickTimer.Interval = TimeSpan.FromMilliseconds(value);
        }

        public static bool IsShown
        {
            get;
            private set;
        }

        public static void Show()
        {
            if (DurationMs != 0)
            {
                m_TickTimer.Stop();
                m_TickTimer.Start();
            }
            IsShown = true;
            m_window.Show();
        }

        public static void Hide()
        {
            IsShown = false;
            m_window.HideAnimated();
        }

        private static void TimerTick(object sender, EventArgs e)
        {
            Hide();
            m_TickTimer.Stop();
        }

        private static void UpdateOsd()
        {
            if (IsVoicemeeterWindowForeground()) return;

            if (!IsShown)
            {
                ApplyVisibilityToOsdElements(Visibility.Collapsed);
            }
            UpdateVmParams();
            Show();
        }

        private static void UpdateVmParams()
        {
            var len = m_vmParams.Length;
            for (int i = 0; i < len; i++)
            {
                m_vmParams[i].Read();
            }
        }

        private static void RefillOsd(VoicemeeterType type)
        {
            m_wpfControl.MainContent.Children.Clear();
            m_vmParams = Array.Empty<VoicemeeterParameter>();
            OsdContentFactory.FillOsdWindow(ref m_wpfControl, ref m_vmParams, type);
            ApplyVisibilityToOsdElements(Visibility.Collapsed);
        }

        private static bool IsVoicemeeterWindowForeground()
        {
            const string WindowClass = "VBCABLE0Voicemeeter0MainWindow0";
            const string WindowText = "VoiceMeeter";

            IntPtr hWnd = GetForegroundWindow();
            return (hWnd != IntPtr.Zero) && 
                (GetWindowClassName(hWnd) == WindowClass) && 
                (GetWindowText(hWnd) == WindowText);
        }

        private static void ApplyVisibilityToOsdElements(Visibility vis)
        {
            m_wpfControl.AllowAutoUpdateSeparators = false;

            var children = m_wpfControl.MainContent.Children;
            foreach (StripControl strip in children)
            {
                strip.Visibility = vis;
                strip.FaderCont.Visibility = vis;
                foreach (ButtonContainer btnCont in strip.BusBtnsContainer.Children)
                {
                    btnCont.Visibility = vis;
                }
                foreach (ButtonContainer btnCont in strip.ControlBtnsContainer.Children)
                {
                    btnCont.Visibility = vis;
                }
            }

            m_wpfControl.UpdateSeparators();
            m_wpfControl.AllowAutoUpdateSeparators = true;
        }

        private static void OnNewVoicemeeterParams(object sender, EventArgs e)
        {
            App.Current.Dispatcher.Invoke(UpdateOsd);
        }

        private static void OnVoicemeeterTypeChange(object sender, VoicemeeterType t)
        {
            App.Current.Dispatcher.Invoke(() => RefillOsd(t));
        }
    }
}
