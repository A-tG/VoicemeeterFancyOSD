using System;
using System.Linq;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using VoicemeeterOsdProgram.Core.Types;
using VoicemeeterOsdProgram.Factories;
using VoicemeeterOsdProgram.Types;
using VoicemeeterOsdProgram.UiControls.OSD;
using VoicemeeterOsdProgram.UiControls.OSD.Strip;
using AtgDev.Voicemeeter.Types;
using VoicemeeterOsdProgram.Interop;
using static TopmostApp.Interop.NativeMethods;

namespace VoicemeeterOsdProgram.Core
{
    public static partial class OsdWindowManager
    {
        private static OsdControl m_wpfControl;
        private static OsdWindow m_window;
        private static DispatcherTimer m_tickTimer;
        private static VoicemeeterParameter[] m_vmParams = Array.Empty<VoicemeeterParameter>();

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

            m_tickTimer = new(DispatcherPriority.Normal);
            m_tickTimer.Interval = TimeSpan.FromMilliseconds(2000);
            m_tickTimer.Tick += TimerTick;

            VoicemeeterApiClient.NewParameters += OnNewVoicemeeterParams;
            VoicemeeterApiClient.ProgramTypeChange += OnVoicemeeterTypeChange;
            VoicemeeterApiClient.Loaded += OnVoicemeeterLoad;
        }

        public static void Init() { }

        public static bool IsEnabled
        {
            get => VoicemeeterApiClient.IsHandlingParams;
            set
            {
                if (value) UpdateVmParams(true);

                VoicemeeterApiClient.IsHandlingParams = value;
            }
        }
        public static double Scale
        {
            get => m_wpfControl.Scale;
            set => m_wpfControl.Scale = value;
        }

        public static double DurationMs
        {
            get => m_tickTimer.Interval.TotalMilliseconds;
            set => m_tickTimer.Interval = TimeSpan.FromMilliseconds(value);
        }

        public static bool IsShown
        {
            get;
            private set;
        }

        public static bool IsIgnoreVmParameters
        {
            get
            {
                return Properties.Settings.Default.IsShowOsdWhenVoicemeeterVisible ? false : IsVoicemeeterWindowForeground();
            }
        }

        public static void Show()
        {
            if (DurationMs != 0)
            {
                m_tickTimer.Stop();
                m_tickTimer.Start();
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
            m_tickTimer.Stop();
        }

        private static void UpdateOsd()
        {
            bool isIgnoreParams = IsIgnoreVmParameters;
            if (!IsShown)
            {
                ApplyVisibilityToOsdElements(Visibility.Collapsed);
            }
            UpdateVmParams(isIgnoreParams);
            if (isIgnoreParams) return;

            UpdateOsdElementsVis();
            Show();
        }

        private static void UpdateVmParams(bool isSkipEvents)
        {
            var len = m_vmParams.Length;
            for (int i = 0; i < len; i++)
            {
                m_vmParams[i].ReadIsIgnoreEvent(isSkipEvents);
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

            IntPtr hWnd = FindWindowEx(IntPtr.Zero, IntPtr.Zero, WindowClass, WindowText);
            return !WindowObstructedHelper.IsObstructed(hWnd); ;
        }

        private static void UpdateOsdElementsVis()
        {
            m_wpfControl.AllowAutoUpdateSeparators = false;

            var children = m_wpfControl.MainContent.Children;
            foreach (StripControl strip in children)
            {
                bool isAnyVisibleBtn = false;
                foreach (ButtonContainer btnCont in strip.BusBtnsContainer.Children)
                {
                    if (btnCont.Visibility == Visibility.Visible)
                    {
                        isAnyVisibleBtn = true;
                        break;
                    }
                }
                foreach (ButtonContainer btnCont in strip.ControlBtnsContainer.Children)
                {
                    if (btnCont.Visibility == Visibility.Visible)
                    {
                        isAnyVisibleBtn = true;
                        break;
                    }
                }

                bool isVisibleChildren = (strip.FaderCont.Visibility == Visibility.Visible) || isAnyVisibleBtn;
                if (isVisibleChildren)
                {
                    strip.Visibility = Visibility.Visible;
                }
            }

            m_wpfControl.UpdateSeparators();
            m_wpfControl.AllowAutoUpdateSeparators = true;
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
            Application.Current.Dispatcher.Invoke(UpdateOsd);
        }

        private static void OnVoicemeeterTypeChange(object sender, VoicemeeterType t)
        {
            Application.Current.Dispatcher.Invoke(() => RefillOsd(t));
        }

        private static void OnVoicemeeterLoad(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => RefillOsd(VoicemeeterApiClient.ProgramType));
        }
    }
}
