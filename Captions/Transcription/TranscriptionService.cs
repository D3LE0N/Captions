using Captions.Cli;
using Captions.Logging;

namespace Captions.Transcription;

/// <summary>
/// Orchestrates the end-to-end transcription of every video in the input folder: locate,
/// extract audio, transcribe, and collect results. Failures on a single video are logged and
/// skipped so one bad file does not abort the whole batch.
/// </summary>
public sealed class TranscriptionService
{
    private readonly IVideoFileLocator _locator;
    private readonly IAudioExtractor _audioExtractor;
    private readonly ITranscriber _transcriber;
    private readonly IAppLogger _logger;

    public TranscriptionService(
        IVideoFileLocator locator,
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
        var videos = _locator.Locate(options.InputDirectory);
        if (videos.Count == 0)
        {
            _logger.Warn($"No supported video files were found in '{options.InputDirectory}'.");
            return [];
        }

        _logger.Info($"Found {videos.Count} video(s) to transcribe.");
        var results = new List<TranscriptionResult>(videos.Count);

        for (var i = 0; i < videos.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var video = videos[i];
            _logger.Info($"[{i + 1}/{videos.Count}] Transcribing '{video.Title}'...");

            try
            {
                var text = await TranscribeSingleAsync(video, cancellationToken);
                results.Add(new TranscriptionResult
                {
                    VideoTitle = video.Title,
                    VideoPath = video.Path,
                    Text = text,
                });
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.Error($"Failed to transcribe '{video.Title}': {ex.Message}");
            }
        }

        return results;
    }

    private async Task<string> TranscribeSingleAsync(VideoFile video, CancellationToken cancellationToken)
    {
        var wavPath = await _audioExtractor.ExtractToWavAsync(video.Path, cancellationToken);
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
