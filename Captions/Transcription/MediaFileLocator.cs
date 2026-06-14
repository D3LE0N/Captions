namespace Captions.Transcription;

/// <summary>
/// Locates media by matching a fixed set of common video and audio container extensions.
/// ffmpeg extracts a Whisper-compatible track from either kind, so both are transcribed the
/// same way.
/// </summary>
public sealed class MediaFileLocator : IMediaFileLocator
{
    private static readonly HashSet<string> MediaExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        // Video containers.
        ".mp4", ".mov", ".mkv", ".avi", ".webm", ".m4v", ".flv", ".wmv", ".mpg", ".mpeg",
        // Audio containers.
        ".mp3", ".wav", ".m4a", ".flac", ".ogg", ".oga", ".opus", ".aac", ".wma", ".aiff", ".aif",
    };

    public IReadOnlyList<MediaFile> Locate(string directory)
    {
        return Directory
            .EnumerateFiles(directory)
            .Where(path => MediaExtensions.Contains(Path.GetExtension(path)))
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .Select(path => new MediaFile(path, Path.GetFileNameWithoutExtension(path)))
            .ToList();
    }
}
