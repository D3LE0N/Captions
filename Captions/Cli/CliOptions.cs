namespace Captions.Cli;

/// <summary>
/// Immutable, validated set of options that drive a single transcription run.
/// </summary>
public sealed class CliOptions
{
    /// <summary>Folder that contains the videos to transcribe (from <c>--dir</c>).</summary>
    public required string InputDirectory { get; init; }

    /// <summary>Folder where the transcription output is written (from <c>--out-dir</c>).</summary>
    public required string OutputDirectory { get; init; }

    /// <summary>Name of the Whisper model to use (from <c>--model</c>), e.g. "base" or "small".</summary>
    public required string ModelName { get; init; }

    /// <summary>When true, write one transcript file per video (from <c>--out-each</c>).</summary>
    public bool WriteEachTranscript { get; init; }

    /// <summary>
    /// When true, write a single combined Markdown file with every video title and its
    /// transcription. Enabled by default; disabled with <c>--no-main</c>.
    /// </summary>
    public bool WriteMainFile { get; init; } = true;
}
