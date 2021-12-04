using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoicemeeterOsdProgram.Options
{
    public class UpdaterOptions : OptionsBase
    {
        private bool m_checkOnStartup = true;

        public bool CheckOnStartup
        {
            get => m_checkOnStartup;
            set => HandlePropertyChange(ref m_checkOnStartup, ref value, CheckOnStartupChanged);
        }

        public event EventHandler<bool> CheckOnStartupChanged;
    }
}
