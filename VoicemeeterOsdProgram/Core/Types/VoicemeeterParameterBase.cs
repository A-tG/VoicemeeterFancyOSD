using AtgDev.Voicemeeter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoicemeeterOsdProgram.Core.Types
{
    public abstract class VoicemeeterParameterBase<T> : IVmParamReadable
    {
        protected readonly string m_command;
        protected readonly RemoteApiExtender m_api;
        protected bool m_isInit;
        protected T m_value; // can be null, initialize in derived class

        public VoicemeeterParameterBase(RemoteApiExtender api, string command)
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
        public abstract int GetParameter(out T value);
        public abstract int SetParameter(T value);

        public void ReadNotifyChanges()
        {
            ReadIsNotifyChanges(true);
        }

        public void Read()
        {
            ReadIsNotifyChanges(false);
        }

        public void ReadIsNotifyChanges(bool isNotify)
        {
            if ((m_api is null) || string.IsNullOrEmpty(m_command)) return;

            if (GetParameter(out T val) == 0)
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
            ValOldNew<T> values = new(oldVal, newVal);
            ReadValueChanged?.Invoke(this, values);
        }

        protected void OnValueRead(T oldVal, T newVal)
        {
            ValOldNew<T> values = new(oldVal, newVal);
            ValueRead?.Invoke(this, values);
        }
    }
}
