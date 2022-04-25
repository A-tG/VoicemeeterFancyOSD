using System;
using System.ComponentModel;

namespace VoicemeeterOsdProgram.Options
{
    public class ProgramOptions : OptionsBase
    {
        private bool m_autostart = false;

        [Description(@"May appear as ""Application Frame Host"" in Startup tab of the Task Manager")]
        public bool Autostart
        {
            get => m_autostart;
            set => HandlePropertyChange(ref m_autostart, ref value, AutostartChanged);
        }

        public event EventHandler<bool> AutostartChanged;
    }
}
