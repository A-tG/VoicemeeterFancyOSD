using System.Collections.Generic;

namespace VoicemeeterOsdProgram.Updater.Types;

public class Release
{

    public string TagName { get; set; } = "";
    public string Name { get; set; } = "";
    public string Body { get; set; } = "";

    public List<Asset> Assets { get; set; } = [];
}
