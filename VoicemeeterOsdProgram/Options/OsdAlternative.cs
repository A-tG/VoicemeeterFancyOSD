using System;
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

        public event EventHandler<bool> EnabledChanged;
    }
}
