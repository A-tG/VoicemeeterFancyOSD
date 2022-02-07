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
        OsNotFound,
        ReleasesNotFound,
        UpdateFailed,
        DownloadFailed,
        ArchiveExtractionFailed,
        Canceled
    }
}
