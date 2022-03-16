using System;
using System.Collections.Generic;
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
        private HashSet<StripElements> m_alwaysShowElements = new();

        public OsdOptions()
        {
            m_alwaysShowElements.Add(StripElements.None);
        }

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

        [Description("Always show these elements on any Strip change. Values can be combined by using comma")]
        public HashSet<StripElements> AlwaysShowElements
        {
            get => m_alwaysShowElements;
            set
            {
                if ((value.Count > 1) && value.Contains(StripElements.None))
                {
                    value.Remove(StripElements.None);
                }
                if (value.Count == 0)
                {
                    value.Add(StripElements.None);
                }

                HandlePropertyChange(ref m_alwaysShowElements, ref value, AlwaysShowElementsChanged);
            }
        }

        public override IEnumerable<KeyValuePair<string, string>> ToDict()
        {
            Dictionary<string, string> list = new(base.ToDict());
            list.Add(nameof(DontShowIfVoicemeeterVisible), DontShowIfVoicemeeterVisible.ToString());
            list.Add(nameof(IsInteractable), IsInteractable.ToString());
            list.Add(nameof(DurationMs), DurationMs.ToString());
            list.Add(nameof(BackgroundOpacity), BackgroundOpacity.ToString());

            string showElements = string.Join(", ", AlwaysShowElements);
            list.Add(nameof(AlwaysShowElements), showElements);
            return list;
        }

        public override void FromDict(Dictionary<string, string> list)
        {
            base.FromDict(list);
            List<string> names = new();
            names.Add(nameof(DontShowIfVoicemeeterVisible));
            names.Add(nameof(HorizontalAlignment));
            names.Add(nameof(HorizontalAlignment));
            names.Add(nameof(BackgroundOpacity));
            foreach (var n in names)
            {
                if (list.ContainsKey(n))
                {
                    TryParseFrom(n, list[n]);
                }
            }

            var name = nameof(AlwaysShowElements);
            if (list.ContainsKey(name))
            {
                HashSet<StripElements> alwaysShow = new(ParseEnumerableFrom<StripElements>(list[name], ","));
                AlwaysShowElements = alwaysShow;
            }
        }

        public event EventHandler<bool> DontShowIfVoicemeeterVisibleChanged;
        public event EventHandler<bool> IsInteractableChanged;
        public event EventHandler<uint> DurationMsChanged;
        public event EventHandler<double> BackgroundOpacityChanged;
        public event EventHandler<HashSet<StripElements>> AlwaysShowElementsChanged;
    }
}
