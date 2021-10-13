using AtgDev.Voicemeeter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoicemeeterOsdProgram.Core.Types
{
    public abstract class VoicemeeterParameterBase
    {
        protected readonly string m_command;
        protected readonly RemoteApiExtender m_api;
        protected bool m_isInit;

        public VoicemeeterParameterBase(RemoteApiExtender api, string command)
        {
            m_api = api;
            m_command = command;
        }

        public bool IsEnabled { get; set; } = true;

        public void ReadNotifyChanges()
        {
            ReadIsNotifyChanges(true);
        }

        public void Read()
        {
            ReadIsNotifyChanges(false);
        }

        public abstract void ReadIsNotifyChanges(bool isNotify);
    }
}
