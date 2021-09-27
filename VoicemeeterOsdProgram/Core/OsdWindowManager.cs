using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using VoicemeeterOsdProgram.Factories;
using VoicemeeterOsdProgram.Types;
using VoicemeeterOsdProgram.UiControls.OSD;
using VoicemeeterOsdProgram.UiControls.OSD.Strip;
using static VoicemeeterOsdProgram.Interop.NativeMethods;

namespace VoicemeeterOsdProgram.Core
{
    public static class OsdWindowManager
    {
        private static OsdControl m_wpfControl;
        private static OsdWindow m_window;
        private static DispatcherTimer m_TickTimer;

        static OsdWindowManager()
        {
            OsdControl osd = new();
            OsdContentFactory.FillOsdWindow(ref osd, AtgDev.Voicemeeter.Types.VoicemeeterType.Potato);
            osd.Background.Opacity = 0.9;
            m_wpfControl = osd;

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
        }

        public static void Init() { }

        public static bool IsEnabled
        {
            get => VoicemeeterApiClient.IsHandlingParams;
            set
            {
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
            get => m_TickTimer.Interval.TotalMilliseconds;
            set => m_TickTimer.Interval = TimeSpan.FromMilliseconds(value);
        }

        public static void Show()
        {
            if (DurationMs != 0)
            {
                m_TickTimer.Stop();
                m_TickTimer.Start();
            }
            m_window.Show();
        }

        public static void Hide()
        {
            m_window.HideAnimated();
        }

        private static void TimerTick(object sender, EventArgs e)
        {
            Hide();
            m_TickTimer.Stop();
        }

        private static void UpdateOsd()
        {
            if (!IsVoicemeeterWindowForeground())
            {
                // Update OSD Content here
                RandomizeElementsState(); // Just for demonstration/test purpose
                Show();
            }
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

        private static void OnNewVoicemeeterParams(object sender, EventArgs e)
        {
            App.Current.Dispatcher.Invoke(UpdateOsd);
        }

        internal static void RandomizeElementsState()
        {
            m_wpfControl.AllowAutoUpdateSeparators = false;
            var random = new Random();
            var children = m_wpfControl.MainContent.Children;
            foreach (StripControl strip in m_wpfControl.MainContent.Children)
            {
                if (random.Next(100) < 50)
                {
                    strip.Visibility = Visibility.Collapsed;
                    continue;
                }
                else
                {
                    strip.Visibility = Visibility.Visible;
                }
                if (random.Next(100) < 50)
                {
                    strip.FaderCont.Visibility = Visibility.Collapsed;
                }
                else
                {
                    strip.FaderCont.Visibility = Visibility.Visible;
                    strip.FaderCont.Fader.Value = random.Next(72) - 60;
                }
                foreach (ButtonContainer btnCont in strip.BusBtnsContainer.Children)
                {
                    if (random.Next(100) < 75)
                    {
                        btnCont.Btn.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        btnCont.Btn.Visibility = Visibility.Visible;
                        if (random.Next(100) < 50)
                        {
                            btnCont.Btn.State++;
                        }
                    }
                }
                foreach (ButtonContainer btnCont in strip.ControlBtnsContainer.Children)
                {
                    if (random.Next(100) < 25)
                    {
                        btnCont.Btn.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        btnCont.Btn.Visibility = Visibility.Visible;
                        if (random.Next(100) < 50)
                        {
                            btnCont.Btn.State++;
                        }
                    }
                }
                m_wpfControl.UpdateSeparators();
                m_wpfControl.AllowAutoUpdateSeparators = true;
            }
        }
    }
}
