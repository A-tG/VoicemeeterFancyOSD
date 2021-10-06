﻿using System;
using VoicemeeterOsdProgram.Options;

namespace VoicemeeterOsdProgram.Core
{
    partial class OsdWindowManager
    {
        public static bool IsEnabled
        {
            get => VoicemeeterApiClient.IsHandlingParams;
            set
            {
                if (value) UpdateVmParams(true);

                VoicemeeterApiClient.IsHandlingParams = value;
            }
        }

        public static bool IsInteractable
        {
            get => m_wpfControl.IsInteractable;
            set
            {
                if (m_wpfControl.IsInteractable == value) return;

                m_window.IsClickThrough = !value;
                m_wpfControl.IsInteractable = value;
            }
        }

        public static double BgOpacity
        {
            get => m_wpfControl.Background.Opacity;
            set
            {
                if (value >= 0)
                {
                    m_wpfControl.Background.Opacity = value;
                }
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
            set
            {
                if (value >= 0)
                {
                    m_tickTimer.Interval = TimeSpan.FromMilliseconds(value);
                }
            }
        }

        public static bool IsShown
        {
            get;
            private set;
        }

        public static bool IsIgnoreVmParameters =>
            m_isMouseEntered || OptionsStorage.Osd.IsShowOnlyIfVoicemeeterHidden && IsVoicemeeterWindowForeground();
    }
}