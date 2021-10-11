﻿using System;
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

namespace VoicemeeterOsdProgram.Core
{
    public static partial class OsdWindowManager
    {
        private static OsdControl m_wpfControl;
        private static OsdWindow m_window;
        private static DispatcherTimer m_displayDurationTimer;
        private static bool m_isMouseEntered;
        private static bool m_isRefillingOsdContent;
        private static IVmParamReadable[] m_vmParams = Array.Empty<IVmParamReadable>();

        static OsdWindowManager()
        {
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

            m_displayDurationTimer = new(DispatcherPriority.Normal);
            m_displayDurationTimer.Interval = TimeSpan.FromMilliseconds(OptionsStorage.Osd.DurationMs);
            m_displayDurationTimer.Tick += TimerTick;

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
            if (IsShown) return;

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
            if (m_isRefillingOsdContent) return;

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

        private static void RefillOsd(VoicemeeterType type)
        {
            m_isRefillingOsdContent = true;

            ApplyVisibilityToOsdElements(Visibility.Collapsed);
            m_wpfControl.MainContent.Children.Clear();
            m_vmParams = Array.Empty<IVmParamReadable>();
            OsdContentFactory.FillOsdWindow(ref m_wpfControl, ref m_vmParams, type);
            ApplyVisibilityToOsdElements(Visibility.Collapsed);

            m_isRefillingOsdContent = false;
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

        private static void OnNewVoicemeeterParams(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(UpdateOsd);
        }

        private static void OnVoicemeeterTypeChange(object sender, VoicemeeterType t)
        {
            if (t == VoicemeeterType.None) return;

            Application.Current.Dispatcher.Invoke(() => RefillOsd(t));
        }

        private static void OnVoicemeeterLoad(object sender, EventArgs e)
        {
            VoicemeeterApiClient.ProgramTypeChange += OnVoicemeeterTypeChange;
            VoicemeeterApiClient.NewParameters += OnNewVoicemeeterParams;
            var type = VoicemeeterApiClient.ProgramType;
            if (type == VoicemeeterType.None) return;

            Application.Current.Dispatcher.Invoke(() => RefillOsd(type));
        }

        private static void OnMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            m_isMouseEntered = true;
            m_displayDurationTimer.Stop();
        }

        private static void OnMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            m_isMouseEntered = false;
            if (IsShown)
            {
                ResetShowTimer();
            }
        }

        private static void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            if (!IsShown) return;

            Hide(75);
        }
    }
}
