using Captions.Cli;
using Captions.Logging;

namespace Captions.Transcription;

/// <summary>
/// Orchestrates the end-to-end transcription of every media file in the input folder: locate,
/// extract audio, transcribe, and collect results. Failures on a single file are logged and
/// skipped so one bad file does not abort the whole batch.
/// </summary>
public sealed class TranscriptionService
{
    private readonly IMediaFileLocator _locator;
    private readonly IAudioExtractor _audioExtractor;
    private readonly ITranscriber _transcriber;
    private readonly IAppLogger _logger;

    public TranscriptionService(
        IMediaFileLocator locator,
        IAudioExtractor audioExtractor,
        ITranscriber transcriber,
        IAppLogger logger)
    {
        _locator = locator;
        _audioExtractor = audioExtractor;
        _transcriber = transcriber;
        _logger = logger;
    }

    public async Task<IReadOnlyList<TranscriptionResult>> TranscribeAllAsync(
        CliOptions options,
        CancellationToken cancellationToken)
    {
        var mediaFiles = _locator.Locate(options.InputDirectory);
        if (mediaFiles.Count == 0)
        {
            _logger.Warn($"No supported media files were found in '{options.InputDirectory}'.");
            return [];
        }

        _logger.Info($"Found {mediaFiles.Count} file(s) to transcribe.");
        var results = new List<TranscriptionResult>(mediaFiles.Count);

        for (var i = 0; i < mediaFiles.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var media = mediaFiles[i];
            _logger.Info($"[{i + 1}/{mediaFiles.Count}] Transcribing '{media.Title}'...");

            try
            {
                var text = await TranscribeSingleAsync(media, cancellationToken);
                results.Add(new TranscriptionResult
                {
                    Title = media.Title,
                    SourcePath = media.Path,
                    Text = text,
                });
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.Error($"Failed to transcribe '{media.Title}': {ex.Message}");
            }
        }

        return results;
    }

    private async Task<string> TranscribeSingleAsync(MediaFile media, CancellationToken cancellationToken)
    {
        var wavPath = await _audioExtractor.ExtractToWavAsync(media.Path, cancellationToken);
        try
        {
            return await _transcriber.TranscribeAsync(wavPath, cancellationToken);
        }
        finally
        {
            TryDeleteTempFile(wavPath);
        }
    }

    private void TryDeleteTempFile(string path)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch (IOException ex)
        {
            _logger.Warn($"Could not delete temporary file '{path}': {ex.Message}");
        }
    }
}
