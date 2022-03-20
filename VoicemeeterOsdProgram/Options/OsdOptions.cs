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
        private HashSet<uint> m_ignoreStripsIndexes = new();

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

        [Description("Always show these elements on any Strip change. Multiple values separated by commas. Example: AlwaysShowElements = Mute, Buses")]
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

        [Description("Dont show changes from Inputs or Outputs with these indexes. Numbering is zero-based. Multiple value separated by commas. Example: IgnoreStripsIndexes = 0, 5, 12")]
        public HashSet<uint> IgnoreStripsIndexes
        {
            get => m_ignoreStripsIndexes;
            set => HandlePropertyChange(ref m_ignoreStripsIndexes, ref value, IgnoreStripsIndexesChanged);
        }

        public override IEnumerable<KeyValuePair<string, string>> ToDict()
        {
            Dictionary<string, string> list = new(ToDictSimpleTypesAuto());

            list.Add(nameof(AlwaysShowElements), string.Join(", ", AlwaysShowElements));
            list.Add(nameof(IgnoreStripsIndexes), string.Join(", ", IgnoreStripsIndexes));
            return list;
        }

        public override void FromDict(Dictionary<string, string> dict)
        {
            base.FromDict(dict);

            if (dict.TryGetValue(nameof(AlwaysShowElements), out string val))
            {
                AlwaysShowElements = new(ParseEnumerableFrom<StripElements>(val, ","));
            }

            if (dict.TryGetValue(nameof(IgnoreStripsIndexes), out val))
            {
                IgnoreStripsIndexes = new(ParseEnumerableFrom<uint>(val, ","));
            }
        }

        public event EventHandler<bool> DontShowIfVoicemeeterVisibleChanged;
        public event EventHandler<bool> IsInteractableChanged;
        public event EventHandler<uint> DurationMsChanged;
        public event EventHandler<double> BackgroundOpacityChanged;
        public event EventHandler<HashSet<StripElements>> AlwaysShowElementsChanged;
        public event EventHandler<HashSet<uint>> IgnoreStripsIndexesChanged;
    }
}
