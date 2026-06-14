namespace Captions.Transcription;

/// <summary>The transcribed text for a single video.</summary>
public sealed class TranscriptionResult
{
    public required string VideoTitle { get; init; }

    public required string VideoPath { get; init; }

    public required string Text { get; init; }
}
