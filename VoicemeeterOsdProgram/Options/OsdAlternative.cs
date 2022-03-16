using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace VoicemeeterOsdProgram.Options
{
    public class OsdAlternative : OsdOptionsBase
    {
        private bool m_enabled = false;

        public OsdAlternative()
        {
            m_displayIndex = 1;
        }

        [Description("Use alternative settings (like display and alignment) for fullscreen apps specified in detect_apps.txt. Useful for for some OpenGL games where OSD is unable to work")]
        public bool Enabled
        {
            get => m_enabled;
            set => HandlePropertyChange(ref m_enabled, ref value, EnabledChanged);
        }

        public override IEnumerable<KeyValuePair<string, string>> ToDict()
        {
            Dictionary<string, string> list = new();
            // this should put "Enabled" first in the config file
            list.Add(nameof(Enabled), Enabled.ToString());

            var oldList = base.ToDict();
            foreach (var kpv in oldList)
            {
                list.Add(kpv.Key, kpv.Value);
            }
            return list;
        }

        public override void FromDict(Dictionary<string, string> list)
        {
            base.FromDict(list);
            var name = nameof(Enabled);
            if (list.ContainsKey(name))
            {
                TryParseFrom(name, list[name]);
            }
        }

        public event EventHandler<bool> EnabledChanged;
    }
}
