using System;
using System.ComponentModel;
using VoicemeeterOsdProgram.Types;

namespace VoicemeeterOsdProgram.Options
{
    public class OsdOptions : OsdOptionsBase
    {
        private bool m_dontShowIfVoicemeeterVisible = true;
        private bool m_isInteractable = false;
        private uint m_durationMs = 2000;
        private double m_backgroundOpacity = 0.9;
        private bool m_alwaysShowMuteBtn = false;
        private bool m_alwaysShowSoloBtn = false;
        private bool m_alwaysShowMonoBtn = false;
        private bool m_alwaysShowBusBtns = false;


        [Description("Dont show OSD if Voicemeeter's window is visible (and not obstructed) or is active window")]
        public bool DontShowIfVoicemeeterVisible 
        {
            get => m_dontShowIfVoicemeeterVisible;
            set => HandlePropertyChange(ref m_dontShowIfVoicemeeterVisible, ref value, DontShowIfVoicemeeterVisibleChanged);
        }

        [Description("User can interact with UI elements in OSD (Gain Fader, Mute, Mono, etc)")]
        public bool IsInteractable 
        {
            get => m_isInteractable;
            set => HandlePropertyChange(ref m_isInteractable, ref value, IsInteractableChanged);
        }

        [Description("How long OSD is displayed (in milliseconds)")]
        public uint DurationMs
        {
            get => m_durationMs;
            set => HandlePropertyChange(ref m_durationMs, ref value, DurationMsChanged);
        }

        [Description("From 0.0 to 1.0. The recommended is 0.8 - 0.95")]
        public double BackgroundOpacity
        {
            get => m_backgroundOpacity;
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                else if (value > 1)
                {
                    value = 1;
                }
                HandlePropertyChange(ref m_backgroundOpacity, ref value, BackgroundOpacityChanged);
            }
        }

        [Description("Always show Mute button on any Strip change")]
        public bool AlwaysShowMuteBtn
        {
            get => m_alwaysShowMuteBtn;
            set => HandlePropertyChange(ref m_alwaysShowMuteBtn, ref value, AlwaysShowMuteBtnChanged);
        }

        [Description("Always show Solo button on any Strip change")]
        public bool AlwaysShowSoloBtn
        {
            get => m_alwaysShowSoloBtn;
            set => HandlePropertyChange(ref m_alwaysShowSoloBtn, ref value, AlwaysShowSoloBtnChanged);
        }

        [Description("Always show Mono button on any Strip change")]
        public bool AlwaysShowMonoBtn
        {
            get => m_alwaysShowSoloBtn;
            set => HandlePropertyChange(ref m_alwaysShowMonoBtn, ref value, AlwaysShowMonoBtnChanged);
        }

        [Description("Always show Hardware Outputs button (A1, A2, B1, B2, etc) on any Strip change")]
        public bool AlwaysShowBusBtns
        {
            get => m_alwaysShowBusBtns;
            set => HandlePropertyChange(ref m_alwaysShowBusBtns, ref value, AlwaysShowBusBtnsChanged);
        }

        public event EventHandler<bool> DontShowIfVoicemeeterVisibleChanged;
        public event EventHandler<bool> IsInteractableChanged;
        public event EventHandler<uint> DurationMsChanged;
        public event EventHandler<double> BackgroundOpacityChanged;
        public event EventHandler<bool> AlwaysShowMuteBtnChanged;
        public event EventHandler<bool> AlwaysShowMonoBtnChanged;
        public event EventHandler<bool> AlwaysShowSoloBtnChanged;
        public event EventHandler<bool> AlwaysShowBusBtnsChanged;
    }
}
