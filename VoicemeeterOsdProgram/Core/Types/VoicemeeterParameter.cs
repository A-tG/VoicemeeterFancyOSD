using AtgDev.Voicemeeter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoicemeeterOsdProgram.Core.Types
{
    public class VoicemeeterParameter<T>
    {
        protected readonly string m_command;
        protected readonly RemoteApiExtender m_api;
        protected bool m_isInit;
        protected T m_value;

        public VoicemeeterParameter(RemoteApiExtender api, string command)
        {
            m_api = api;
            m_command = command;
        }

        public T Value
        {
            get => m_value;
            protected set
            {
                if ((!m_value.Equals(value)) && m_isInit)
                {
                    OnReadValueChanged(m_value, value);
                }
                m_value = value;
            }
        }

        public event EventHandler<ValOldNew<T>> ReadValueChanged;

        private void OnReadValueChanged(T oldVal, T newVal)
        {
            ValOldNew<T> values = new(oldVal, newVal);
            ReadValueChanged?.Invoke(this, values);
        }
    }
}
