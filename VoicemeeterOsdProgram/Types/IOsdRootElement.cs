using System;

namespace VoicemeeterOsdProgram.Types
{
    public interface IOsdRootElement
    {
        public bool HasChangesFlag { get; set; }

        public bool HasAnyChildVisibleFlag { get; set; }

    }
}
