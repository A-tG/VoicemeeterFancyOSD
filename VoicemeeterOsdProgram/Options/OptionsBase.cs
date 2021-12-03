using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoicemeeterOsdProgram.Options
{
    public abstract class OptionsBase
    {
        protected void HandlePropertyChange<T>(ref T oldVal, ref T newVal, EventHandler<T> eventIfNotEqual)
        {
            if (oldVal.Equals(newVal)) return;

            oldVal = newVal;
            eventIfNotEqual?.Invoke(this, newVal);
        }
    }
}
