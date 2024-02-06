using System;

namespace VoicemeeterOsdProgram.Types;

public interface IOsdAnimatedElement
{
    public Func<bool> IsAnimationsEnabled { get; set; }
}
