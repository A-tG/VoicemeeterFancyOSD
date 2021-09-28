using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoicemeeterOsdProgram.Core.Types
{
    public struct ValuesPair<T>
    {
        public T oldVal;
        public T newVal;

        public ValuesPair(T oldVal, T newVal)
        {
            this.oldVal = oldVal;
            this.newVal = newVal;
        }
    }
}
