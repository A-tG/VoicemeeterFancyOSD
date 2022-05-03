using AtgDev.Voicemeeter;
using System;
using System.Runtime.InteropServices;

namespace VoicemeeterOsdProgram.Core.Types
{
    public abstract class VoicemeeterParameterBase
    {
        protected readonly string m_name;
        protected readonly RemoteApiExtender m_api;
        protected bool m_isInit;

        protected readonly IntPtr m_nameBuffer;

        public VoicemeeterParameterBase(RemoteApiExtender api, string command)
        {
            m_api = api;
            m_name = command;
            m_nameBuffer = Marshal.StringToHGlobalAnsi(m_name);

            Read();
        }

        public string Name { get => m_name; }

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

        public abstract void ClearEvents();

        ~VoicemeeterParameterBase()
        {
            Marshal.FreeHGlobal(m_nameBuffer);
        }
    }
}
