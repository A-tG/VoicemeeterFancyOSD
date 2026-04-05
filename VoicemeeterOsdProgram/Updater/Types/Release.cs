using System.Collections.Generic;

namespace VoicemeeterOsdProgram.Updater.Types;

public record Release
{

    public required string TagName { get; set; }
    public string Name { get; set; } = "";
    public string Body { get; set; } = "";

    public required List<Asset> Assets { get; set; }
}
