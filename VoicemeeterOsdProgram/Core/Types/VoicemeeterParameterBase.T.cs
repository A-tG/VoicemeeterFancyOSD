using AtgDev.Voicemeeter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoicemeeterOsdProgram.Core.Types
{
    public abstract class VoicemeeterParameterBase<T> : VoicemeeterParameterBase
    {
        protected T m_value; // can be null, initialize in derived class

        public VoicemeeterParameterBase(RemoteApiExtender api, string command) : base(api, command) { }

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

        public abstract int GetParameter(out T value);

        public abstract int SetParameter(T value);

        public override void ReadIsNotifyChanges(bool isNotify)
        {
            if (!IsEnabled) return;

            if ((m_api is null) || string.IsNullOrEmpty(m_command)) return;

            var res = GetParameter(out T val);
            if (res == 0)
            {
                var oldVal = m_value;
                if (isNotify)
                {
                    Value = val;
                }
                else
                {
                    m_value = val;
                }
                m_isInit = true;
                OnValueRead(oldVal, val);
            }
        }

        public void Write(T value)
        {
            if (!IsEnabled) return;

            if ((m_api is null) || string.IsNullOrEmpty(m_command)) return;

            if (SetParameter(value) == 0)
            {
                m_value = value;
            }
        }

        public event EventHandler<ValOldNew<T>> ReadValueChanged;
        public event EventHandler<ValOldNew<T>> ValueRead;

        private void OnReadValueChanged(T oldVal, T newVal)
        {
            if (!IsEnabled) return;

            ValOldNew<T> values = new(oldVal, newVal);
            ReadValueChanged?.Invoke(this, values);
        }

        protected void OnValueRead(T oldVal, T newVal)
        {
            if (!IsEnabled) return;

            ValOldNew<T> values = new(oldVal, newVal);
            ValueRead?.Invoke(this, values);
        }
    }
}
