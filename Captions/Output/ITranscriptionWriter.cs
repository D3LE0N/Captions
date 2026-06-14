using Captions.Cli;
using Captions.Transcription;

namespace Captions.Output;

/// <summary>
/// Writes transcription results to disk. Implementations represent the different output
/// strategies (combined Markdown file, one file per media file) and are selected by the options.
/// </summary>
public interface ITranscriptionWriter
{
    Task WriteAsync(
        IReadOnlyList<TranscriptionResult> results,
        CliOptions options,
        CancellationToken cancellationToken);
}
