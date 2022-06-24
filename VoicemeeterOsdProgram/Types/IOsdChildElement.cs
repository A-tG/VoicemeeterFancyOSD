using System;

namespace VoicemeeterOsdProgram.Types
{
    public interface IOsdChildElement
    {
        public IOsdRootElement OsdParent { get; set; }

        public Func<bool> IsAlwaysVisible { get; set; }

        public Func<bool> IsNeverShow { get; set; }
    }
}
