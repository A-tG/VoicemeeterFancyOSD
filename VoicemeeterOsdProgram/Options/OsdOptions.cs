using System;
using VoicemeeterOsdProgram.Types;

namespace VoicemeeterOsdProgram.Options
{
    public class OsdOptions
    {
        private uint m_displayIndex;
        private bool m_isShowOnlyIfVoicemeeterHidden = true;
        private bool m_isInteractable = false;
        private uint m_durationMs = 2000;
        private double m_backgroundOpacity = 0.9;
        private bool m_isBackgroundBlurred = true;
        private HorAlignment m_horizontalAlignment = HorAlignment.Right;
        private VertAlignment m_verticalAlignment = VertAlignment.Top;

        public uint DisplayIndex
        {
            get => m_displayIndex;
            set
            {
                if (value == m_displayIndex) return;

                m_displayIndex = value;
                DisplayIndexChanged?.Invoke(this, value);
            }
        }

        public bool IsShowOnlyIfVoicemeeterHidden 
        {
            get => m_isShowOnlyIfVoicemeeterHidden;
            set
            {
                if (value == m_isShowOnlyIfVoicemeeterHidden) return;

                m_isShowOnlyIfVoicemeeterHidden = value;
                IsShowOnlyIfVoicemeeterHiddenChanged?.Invoke(this, value);
            }
        }

        public bool IsInteractable 
        {
            get => m_isInteractable;
            set
            {
                if (value == m_isInteractable) return;

                m_isInteractable = value;
                IsInteractableChanged?.Invoke(this, value);
            }
        }

        public uint DurationMs
        {
            get => m_durationMs;
            set
            {
                if (value == m_durationMs) return;

                m_durationMs = value;
                DurationMsChanged?.Invoke(this, value);
            }
        }

        public double BackgroundOpacity
        {
            get => m_backgroundOpacity;
            set
            {
                if (value == m_backgroundOpacity) return;

                m_backgroundOpacity = value;
                BackgroundOpacityChanged?.Invoke(this, value);
            }
        }

        public bool IsBackgroundBlurred
        {
            get => m_isBackgroundBlurred;
            set
            {
                if (value == m_isBackgroundBlurred) return;

                m_isBackgroundBlurred = value;
                IsBackgroundBlurredChanged?.Invoke(this, value);
            }
        }

        public HorAlignment HorizontalAlignment
        {
            get => m_horizontalAlignment;
            set
            {
                if (value == m_horizontalAlignment) return;

                m_horizontalAlignment = value;
                HorizontalAlignmentChanged?.Invoke(this, value);
            }
        }

        public VertAlignment VerticalAlignment
        {
            get => m_verticalAlignment;
            set
            {
                if (value == m_verticalAlignment) return;

                m_verticalAlignment = value;
                VerticalAlignmentChanged?.Invoke(this, value);
            }
        }

        public event EventHandler<uint> DisplayIndexChanged;
        public event EventHandler<bool> IsShowOnlyIfVoicemeeterHiddenChanged;
        public event EventHandler<bool> IsInteractableChanged;
        public event EventHandler<uint> DurationMsChanged;
        public event EventHandler<double> BackgroundOpacityChanged;
        public event EventHandler<bool> IsBackgroundBlurredChanged;
        public event EventHandler<HorAlignment> HorizontalAlignmentChanged;
        public event EventHandler<VertAlignment> VerticalAlignmentChanged;
    }
}
