namespace VoicemeeterOsdProgram.Updater.Types
{
    public enum UpdaterResult
    {
        Error,
        Updated,
        Downloaded,
        Unpacked,
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
