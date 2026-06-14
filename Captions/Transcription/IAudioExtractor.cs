namespace Captions.Transcription;

/// <summary>Extracts a Whisper-compatible audio track from a video or audio file.</summary>
public interface IAudioExtractor
{
    /// <summary>
    /// Extracts the audio of <paramref name="inputPath"/> to a temporary 16 kHz mono PCM WAV
    /// file (the format Whisper expects) and returns its path. The caller owns the returned
    /// file and is responsible for deleting it.
    /// </summary>
    Task<string> ExtractToWavAsync(string inputPath, CancellationToken cancellationToken);
}
