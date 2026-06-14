using Captions.Cli;
using Captions.Logging;
using Captions.Output;
using Captions.Transcription;

return await CaptionsApp.RunAsync(args);

/// <summary>
/// Composition root for the captions CLI. Wires the concrete services together, handles
/// argument parsing and top-level error reporting, and returns a process exit code.
/// </summary>
internal static class CaptionsApp
{
    private const int ExitSuccess = 0;
    private const int ExitUsageError = 1;
    private const int ExitRuntimeError = 2;
    private const int ExitCancelled = 130;

    public static async Task<int> RunAsync(string[] args)
    {
        var logger = new ConsoleLogger();

        var parseResult = new CliParser().Parse(args);
        if (parseResult.HelpRequested)
        {
            Console.WriteLine(HelpText.Build());
            return ExitSuccess;
        }

        if (parseResult.Error is not null)
        {
            logger.Error(parseResult.Error);
            return ExitUsageError;
        }

        var options = parseResult.Options!;
        if (!Directory.Exists(options.InputDirectory))
        {
            logger.Error($"Input directory does not exist: '{options.InputDirectory}'.");
            return ExitUsageError;
        }

        Directory.CreateDirectory(options.OutputDirectory);

        using var cancellation = new CancellationTokenSource();
        Console.CancelKeyPress += (_, eventArgs) =>
        {
            eventArgs.Cancel = true;
            cancellation.Cancel();
        };

        try
        {
            await ExecuteAsync(options, logger, cancellation.Token);
            return ExitSuccess;
        }
        catch (OperationCanceledException)
        {
            logger.Warn("Cancelled.");
            return ExitCancelled;
        }
        catch (Exception ex)
        {
            logger.Error(ex.Message);
            return ExitRuntimeError;
        }
    }

    private static async Task ExecuteAsync(CliOptions options, IAppLogger logger, CancellationToken cancellationToken)
    {
        var appDirectory = AppContext.BaseDirectory;

        IFfmpegProvider ffmpegProvider = new FfmpegProvider(Path.Combine(appDirectory, "tools"), logger);
        var ffmpegPath = await ffmpegProvider.EnsureAvailableAsync(cancellationToken);

        // The model name was validated during parsing, so resolution always succeeds here.
        WhisperModelCatalog.TryResolve(options.ModelName, out var modelType);
        IWhisperModelProvider modelProvider =
            new WhisperModelProvider(Path.Combine(appDirectory, "models"), logger, modelType);
        var modelPath = await modelProvider.EnsureModelAsync(cancellationToken);

        using var transcriber = new WhisperTranscriber(modelPath);
        var service = new TranscriptionService(
            new VideoFileLocator(),
            new FfmpegAudioExtractor(ffmpegPath),
            transcriber,
            logger);

        var results = await service.TranscribeAllAsync(options, cancellationToken);
        if (results.Count == 0)
        {
            logger.Warn("No transcriptions were produced; nothing to write.");
            return;
        }

        foreach (var writer in BuildWriters(options, logger))
        {
            await writer.WriteAsync(results, options, cancellationToken);
        }
    }

    private static IEnumerable<ITranscriptionWriter> BuildWriters(CliOptions options, IAppLogger logger)
    {
        if (options.WriteMainFile)
            yield return new MainMarkdownWriter(logger);

        if (options.WriteEachTranscript)
            yield return new PerVideoWriter(logger);
    }
}
