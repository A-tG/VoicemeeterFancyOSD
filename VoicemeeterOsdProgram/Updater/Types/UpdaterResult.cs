namespace VoicemeeterOsdProgram.Updater.Types
{
    public enum UpdaterResult
    {
        Error,
        Updated,
        NewVersionFound,
        VersionUpToDate,
        ConnectionError,
        ArchitectureNotFound,
        ReleasesNotFound,
        UpdateFailed,
        DownloadFailed,
        ArchiveExtractionFailed,
        Canceled
    }
}
