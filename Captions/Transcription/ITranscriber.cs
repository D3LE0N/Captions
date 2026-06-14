namespace Captions.Transcription;

/// <summary>Transcribes a prepared WAV audio file into plain text.</summary>
public interface ITranscriber
{
    /// <summary>Transcribes the 16 kHz mono WAV at <paramref name="wavPath"/> into text.</summary>
    Task<string> TranscribeAsync(string wavPath, CancellationToken cancellationToken);
}
