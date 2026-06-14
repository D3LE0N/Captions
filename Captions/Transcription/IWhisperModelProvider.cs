namespace Captions.Transcription;

/// <summary>Guarantees that the Whisper GGML model file is available locally.</summary>
public interface IWhisperModelProvider
{
    /// <summary>
    /// Ensures the model exists under the models folder (downloading it on first use) and
    /// returns the absolute path to the model file.
    /// </summary>
    Task<string> EnsureModelAsync(CancellationToken cancellationToken);
}
