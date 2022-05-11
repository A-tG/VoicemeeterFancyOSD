using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoicemeeterOsdProgram.Types
{
    public interface IOsdAnimatedElement
    {
        public Func<bool> IsAnimationsEnabled { get; set; }
    }
}
