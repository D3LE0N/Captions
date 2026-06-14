using System.Text;
using Captions.Cli;
using Captions.Logging;
using Captions.Transcription;

namespace Captions.Output;

/// <summary>
/// Default writer: produces a single Markdown file containing every media title (as a heading)
/// followed by its transcription.
/// </summary>
public sealed class MainMarkdownWriter : ITranscriptionWriter
{
    private const string FileName = "transcriptions.md";

    private readonly IAppLogger _logger;

    public MainMarkdownWriter(IAppLogger logger)
    {
        _logger = logger;
    }

    public async Task WriteAsync(
        IReadOnlyList<TranscriptionResult> results,
        CliOptions options,
        CancellationToken cancellationToken)
    {
        var builder = new StringBuilder();
        builder.AppendLine("# Transcriptions");
        builder.AppendLine();

        foreach (var result in results)
        {
            builder.AppendLine($"## {result.Title}");
            builder.AppendLine();
            builder.AppendLine(result.Text.Length > 0 ? result.Text : "_(no speech detected)_");
            builder.AppendLine();
        }

        var outputPath = Path.Combine(options.OutputDirectory, FileName);
        await File.WriteAllTextAsync(outputPath, builder.ToString(), cancellationToken);
        _logger.Info($"Wrote combined transcript to '{outputPath}'.");
    }
}
