namespace Captions.Transcription;

/// <summary>
/// Guarantees that an ffmpeg binary is available before any audio extraction happens,
/// downloading it on demand.
/// </summary>
public interface IFfmpegProvider
{
    /// <summary>
    /// Ensures ffmpeg is present (downloading it if necessary) and returns the absolute path
    /// to the ffmpeg executable.
    /// </summary>
    Task<string> EnsureAvailableAsync(CancellationToken cancellationToken);
}
