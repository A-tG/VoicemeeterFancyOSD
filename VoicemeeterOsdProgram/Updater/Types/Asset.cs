namespace VoicemeeterOsdProgram.Updater.Types;

public record Asset
{
    public required string Name { get; set; }
    public required string BrowserDownloadUrl { get; set; }
}
