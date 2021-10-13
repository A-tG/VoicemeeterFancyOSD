using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using VoicemeeterOsdProgram.Core.Types;
using VoicemeeterOsdProgram.Factories;
using VoicemeeterOsdProgram.Types;
using VoicemeeterOsdProgram.UiControls.OSD;
using AtgDev.Voicemeeter.Types;
using VoicemeeterOsdProgram.Interop;
using static TopmostApp.Interop.NativeMethods;
using VoicemeeterOsdProgram.Options;
using VoicemeeterOsdProgram.UiControls.OSD.Strip;

namespace VoicemeeterOsdProgram.Core
{
    public static partial class OsdWindowManager
    {
        private const int WaitMsAfterVmStarted = 5000;
        private const int WaitMsAfetVmTypeChange = 11000;

        private static OsdControl m_wpfControl;
        private static OsdWindow m_window;

        private static DispatcherTimer m_displayDurationTimer = new(DispatcherPriority.Normal)
        {
            Interval = TimeSpan.FromMilliseconds(OptionsStorage.Osd.DurationMs)
        };
        private static DispatcherTimer m_WaitForVmStartedTimer = new(DispatcherPriority.Normal) 
        { 
            Interval = TimeSpan.FromMilliseconds(WaitMsAfterVmStarted) 
        };
        private static DispatcherTimer m_WaitForVmTypeTimer = new(DispatcherPriority.Normal)
        {
            Interval = TimeSpan.FromMilliseconds(WaitMsAfetVmTypeChange)
        };

        private static bool m_isMouseEntered;
        private static bool m_changingOsdContent;
        private static bool m_isVmStarting;
        private static bool m_isVmTypeChanging;
        private static VoicemeeterParameterBase[] m_vmParams = Array.Empty<VoicemeeterParameterBase>();

        static OsdWindowManager()
        {
            AppDomain.CurrentDomain.UnhandledException += (_, _) => Exit();
            Application.Current.Exit += (_, _) => Exit();

            OsdControl osd = new();
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

            m_displayDurationTimer.Tick += TimerTick;
            m_WaitForVmStartedTimer.Tick += WaitForVmTimerTick;
            m_WaitForVmTypeTimer.Tick += WaitForVmTypeTimerTick;

            var options = OptionsStorage.Osd;
            IsInteractable = options.IsInteractable;
            BgOpacity = options.BackgroundOpacity;

            options.IsInteractableChanged += (_, val) => IsInteractable = val;
            options.DurationMsChanged += (_, val) => DurationMs = val;
            options.BackgroundOpacityChanged += (_, val) => BgOpacity = val;

            m_wpfControl.CloseBtn.Click += OnCloseButtonClick;
            m_wpfControl.MouseEnter += OnMouseEnter;
            m_wpfControl.MouseLeave += OnMouseLeave;
            
            VoicemeeterApiClient.Loaded += OnVoicemeeterLoad;
        }

        public static void Init() { }

        public static void Show()
        {
            if (!m_isMouseEntered)
            {
                ResetShowTimer();
            }
            if (IsShown || !m_wpfControl.IsAnyVisibleChild()) return;

            IsShown = true;
            m_window.Show();
        }

        public static void ShowFull()
        {
            ApplyVisibilityToOsdElements(Visibility.Visible);
            Show();
        }

        public static void Hide()
        {
            IsShown = false;
            m_window.HideAnimated();
        }

        public static void Hide(uint fadeOutDuration)
        {
            IsShown = false;
            m_window.HideAnimated(fadeOutDuration);
        }

        private static void TimerTick(object sender, EventArgs e)
        {
            m_displayDurationTimer.Stop();
            Hide();
        }

        private static void UpdateOsd()
        {
            if (m_changingOsdContent || m_isVmTypeChanging) return;

            if (m_isVmStarting)
            {
                UpdateVmParams(false);
                return;
            }

            bool isNotifyChanges = !IsIgnoreVmParameters;
            if (!IsShown)
            {
                ApplyVisibilityToOsdElements(Visibility.Collapsed);
            }
            UpdateVmParams(isNotifyChanges);
            if (!isNotifyChanges) return;

            UpdateOsdElementsVis();
            Show();
        }

        private static void UpdateVmParams(bool isNotifyChanges)
        {
            var len = m_vmParams.Length;
            for (int i = 0; i < len; i++)
            {
                m_vmParams[i].ReadIsNotifyChanges(isNotifyChanges);
            }
        }

        private static void ClearOsd()
        {
            var children = m_wpfControl.MainContent.Children;
            // removing elements with x:Name and Name attribute that are used in events
            // in attempt to prevent memory leak
            foreach (StripControl strip in children)
            {
                foreach (ButtonContainer btnCont in strip.BusBtnsContainer.Children)
                {
                    btnCont.Btn = null;
                }
                foreach (ButtonContainer btnCont in strip.ControlBtnsContainer.Children)
                {
                    btnCont.Btn = null;
                }
                strip.StripLabel = null;
                strip.BusBtnsContainer = null;
                strip.ControlBtnsContainer = null;
                strip.FaderCont.Fader = null;
                strip.FaderCont = null;
            }
            children.Clear();
        }

        private static void RefillOsd(VoicemeeterType type)
        {
            if (type == VoicemeeterType.None) return;

            m_changingOsdContent = true;
            ApplyVisibilityToOsdElements(Visibility.Collapsed);

            ClearOsd();
            m_vmParams = Array.Empty<VoicemeeterParameterBase>();
            OsdContentFactory.FillOsdWindow(ref m_wpfControl, ref m_vmParams, type);

            ApplyVisibilityToOsdElements(Visibility.Collapsed);
            m_changingOsdContent = false;
        }

        private static void ResetShowTimer()
        {
            if (DurationMs != 0)
            {
                m_displayDurationTimer.Stop();
                m_displayDurationTimer.Start();
            }
        }

        private static bool IsVoicemeeterWindowForeground()
        {
            const string WindowClass = "VBCABLE0Voicemeeter0MainWindow0";
            const string WindowText = "VoiceMeeter";

            IntPtr hWnd = FindWindowEx(IntPtr.Zero, IntPtr.Zero, WindowClass, WindowText);
            bool isFocused = GetForegroundWindow() == hWnd;
            return isFocused || !WindowObstructedHelper.IsObstructed(hWnd);
        }

        private static void Exit()
        {
            m_displayDurationTimer?.Stop();
            m_WaitForVmStartedTimer?.Stop();
        }
    }
}
