using System;

namespace VoicemeeterOsdProgram.Options
{
    public class OsdOptions
    {
        private bool m_isShowOnlyIfVoicemeeterHidden = true;
        private bool m_isInteractable = false;
        private uint m_durationMs = 2000;
        private double m_backgroundOpacity = 0.9;

        public bool IsShowOnlyIfVoicemeeterHidden 
        {
            get => m_isShowOnlyIfVoicemeeterHidden;
            set
            {
                if (value != m_isShowOnlyIfVoicemeeterHidden)
                {
                    m_isShowOnlyIfVoicemeeterHidden = value;
                    IsShowOnlyIfVoicemeeterHiddenChanged?.Invoke(this, value);
                }
            }
        }

        public bool IsInteractable 
        {
            get => m_isInteractable;
            set
            {
                if (value != m_isInteractable)
                {
                    m_isInteractable = value;
                    IsInteractableChanged?.Invoke(this, value);
                }
            }
        }

        public uint DurationMs
        {
            get => m_durationMs;
            set
            {
                if (value != m_durationMs)
                {
                    m_durationMs = value;
                    DurationMsChanged?.Invoke(this, value);
                }
            }
        }

        public double BackgroundOpacity
        {
            get => m_backgroundOpacity;
            set
            {
                if (value != m_backgroundOpacity)
                {
                    m_backgroundOpacity = value;
                    BackgroundOpacityChanged?.Invoke(this, value);
                }
            }
        }

        public event EventHandler<bool> IsShowOnlyIfVoicemeeterHiddenChanged;
        public event EventHandler<bool> IsInteractableChanged;
        public event EventHandler<uint> DurationMsChanged;
        public event EventHandler<double> BackgroundOpacityChanged;
    }
}
