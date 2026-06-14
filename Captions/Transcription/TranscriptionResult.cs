namespace Captions.Transcription;

/// <summary>The transcribed text for a single media file.</summary>
public sealed class TranscriptionResult
{
    public required string Title { get; init; }

    public required string SourcePath { get; init; }

    public required string Text { get; init; }
}
