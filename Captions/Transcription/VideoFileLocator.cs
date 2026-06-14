namespace Captions.Transcription;

/// <summary>Locates videos by matching a fixed set of common container extensions.</summary>
public sealed class VideoFileLocator : IVideoFileLocator
{
    private static readonly HashSet<string> VideoExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".mp4", ".mov", ".mkv", ".avi", ".webm", ".m4v", ".flv", ".wmv", ".mpg", ".mpeg",
    };

    public IReadOnlyList<VideoFile> Locate(string directory)
    {
        return Directory
            .EnumerateFiles(directory)
            .Where(path => VideoExtensions.Contains(Path.GetExtension(path)))
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .Select(path => new VideoFile(path, Path.GetFileNameWithoutExtension(path)))
            .ToList();
    }
}
