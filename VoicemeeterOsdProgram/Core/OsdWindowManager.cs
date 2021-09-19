using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using VoicemeeterOsdProgram.Factories;
using VoicemeeterOsdProgram.Interop;
using VoicemeeterOsdProgram.Types;
using VoicemeeterOsdProgram.UiControls.OSD;
using VoicemeeterOsdProgram.UiControls.OSD.Strip;
using static VoicemeeterOsdProgram.Interop.NativeMethods;

namespace VoicemeeterOsdProgram.Core
{
    public static class OsdWindowManager
    {
        private const int m_tickMs = 1000 / 30;

        private static OsdControl m_wpfControl;
        private static OsdWindow m_window;
        private static DispatcherTimer m_timer = new();
        private static Stopwatch m_stopWatch = new();
        private static bool m_isIdle = false;

        public static void  Init()
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
                ZBandID = GetZBandID()
            };
            m_window = win;

            m_timer.Interval = TimeSpan.FromMilliseconds(m_tickMs);
            m_timer.Tick += TimerTick;
            m_timer.Start();
        }

        public static double Scale
        {
            get => m_wpfControl.Scale;
            set => m_wpfControl.Scale = value;
        }

        public static uint DurationMs
        {
            get;
            set;
        } = 2000;

        public static void Show()
        {
            m_isIdle = true;
            m_stopWatch.Restart();
            m_window.Show();
        }

        public static void Hide()
        {
            m_isIdle = false;
            m_window.HideAnimated();
        }

        private static ZBandID GetZBandID()
        {
            var zbid = ZBandID.Default;
            using (var proc = Process.GetCurrentProcess())
            {
                var isImmersive = IsImmersiveProcess(proc.Handle);
                var hasUiAccess = HasUiAccessProcess(proc.Handle);
                zbid = isImmersive ? ZBandID.AboveLockUX : hasUiAccess ? ZBandID.UIAccess : ZBandID.Desktop;
            }
            return zbid;
        }

        private static void TimerTick(object sender, EventArgs e)
        {
            // Voicemeeter logic for reading parameters will be here

            if ((!m_isIdle) || (DurationMs == 0)) return;

            if (m_stopWatch.ElapsedMilliseconds >= DurationMs)
            {
                m_stopWatch.Reset();
                Hide();
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
