using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoicemeeterOsdProgram.Options
{
    public class UpdaterOptions : OptionsBase
    {
        private bool m_checkAutomatically = true;

        public bool CheckAutomatically
        {
            get => m_checkAutomatically;
            set => HandlePropertyChange(ref m_checkAutomatically, ref value, CheckAutomaticallyChanged);
        }

        public event EventHandler<bool> CheckAutomaticallyChanged;
    }
}
