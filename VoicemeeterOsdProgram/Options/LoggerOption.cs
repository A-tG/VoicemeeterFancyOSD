using System;
using System.ComponentModel;

namespace VoicemeeterOsdProgram.Options
{
    public class LoggerOption : OptionsBase
    {
        private bool m_enabled = false;
        private uint m_logFilesMax = 5;

        public bool Enabled
        {
            get => m_enabled;
            set => HandlePropertyChange(ref m_enabled, ref value, EnabledChanged);
        }

        [Description("Maximum number of stored Log files (excluding the current). 0 - no limit")]
        public uint LogFilesMax
        {
            get => m_logFilesMax;
            set => HandlePropertyChange(ref m_logFilesMax, ref value, LogFilesMaxChanged);
        }

        public event EventHandler<bool> EnabledChanged;
        public event EventHandler<uint> LogFilesMaxChanged;
    }
}
