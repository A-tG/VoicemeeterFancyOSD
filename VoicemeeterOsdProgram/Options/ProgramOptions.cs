using System;

namespace VoicemeeterOsdProgram.Options
{
    public class ProgramOptions : OptionsBase
    {
        private bool m_autostart = false;

        public bool Autostart
        {
            get => m_autostart;
            set => HandlePropertyChange(ref m_autostart, ref value, AutostartChanged);
        }

        public event EventHandler<bool> AutostartChanged;
    }
}
