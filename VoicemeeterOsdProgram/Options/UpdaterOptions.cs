using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace VoicemeeterOsdProgram.Options
{
    public class UpdaterOptions : OptionsBase
    {
        private bool m_checkOnStartup = false;

        [Description("Check for updates on program startup")]
        public bool CheckOnStartup
        {
            get => m_checkOnStartup;
            set => HandlePropertyChange(ref m_checkOnStartup, ref value, CheckOnStartupChanged);
        }

        public override IEnumerable<KeyValuePair<string, string>> ToDict()
        {
            Dictionary<string, string> list = new();
            list.Add(nameof(CheckOnStartup), CheckOnStartup.ToString());
            return list;
        }

        public override void FromDict(Dictionary<string, string> list)
        {
            var name = nameof(CheckOnStartup);
            if (list.ContainsKey(name)) TryParseFrom(name, list[name]);
        }

        public event EventHandler<bool> CheckOnStartupChanged;
    }
}
