using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoicemeeterOsdProgram.Types
{
    public interface IOsdChildElement
    {
        public IOsdRootElement OsdParent { get; set; }

        public Func<bool> IsAlwaysVisible { get; set; }

        public Func<bool> IsNeverShow { get; set; }
    }
}
