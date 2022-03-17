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

        public event EventHandler<bool> CheckOnStartupChanged;
    }
}
