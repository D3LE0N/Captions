using Captions.Cli;
using Captions.Logging;
using Captions.Transcription;

namespace Captions.Output;

/// <summary>
/// Writes one plain-text transcript file per media file, named after the source. Enabled by the
/// <c>--out-each</c> option.
/// </summary>
public sealed class PerFileWriter : ITranscriptionWriter
{
    private readonly IAppLogger _logger;

    public PerFileWriter(IAppLogger logger)
    {
        _logger = logger;
    }

    public async Task WriteAsync(
        IReadOnlyList<TranscriptionResult> results,
        CliOptions options,
        CancellationToken cancellationToken)
    {
        foreach (var result in results)
        {
            var fileName = $"{SanitizeFileName(result.Title)}.txt";
            var outputPath = Path.Combine(options.OutputDirectory, fileName);
            await File.WriteAllTextAsync(outputPath, result.Text, cancellationToken);
            _logger.Info($"Wrote transcript to '{outputPath}'.");
        }
    }

    /// <summary>Replaces characters that are invalid in file names with an underscore.</summary>
    private static string SanitizeFileName(string title)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var chars = title.Select(c => invalid.Contains(c) ? '_' : c).ToArray();
        return new string(chars);
    }
}
