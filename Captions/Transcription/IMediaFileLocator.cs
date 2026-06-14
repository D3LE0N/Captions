namespace Captions.Transcription;

/// <summary>Finds the media files (video or audio) inside an input directory.</summary>
public interface IMediaFileLocator
{
    /// <summary>
    /// Returns the media files in <paramref name="directory"/>, ordered by name. The result is
    /// empty when no supported media files are present.
    /// </summary>
    IReadOnlyList<MediaFile> Locate(string directory);
}
