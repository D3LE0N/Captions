namespace Captions.Transcription;

/// <summary>Finds the video files inside an input directory.</summary>
public interface IVideoFileLocator
{
    /// <summary>
    /// Returns the video files in <paramref name="directory"/>, ordered by name. The result is
    /// empty when no supported videos are present.
    /// </summary>
    IReadOnlyList<VideoFile> Locate(string directory);
}
