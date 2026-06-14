using Captions.Logging;
using Whisper.net.Ggml;

namespace Captions.Transcription;

/// <summary>
/// Downloads and caches a Whisper GGML model under the models folder next to the executable.
/// </summary>
public sealed class WhisperModelProvider : IWhisperModelProvider
{
    private readonly string _modelsDirectory;
    private readonly GgmlType _modelType;
    private readonly IAppLogger _logger;

    public WhisperModelProvider(string modelsDirectory, IAppLogger logger, GgmlType modelType = GgmlType.Base)
    {
        _modelsDirectory = modelsDirectory;
        _logger = logger;
        _modelType = modelType;
    }

    public async Task<string> EnsureModelAsync(CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(_modelsDirectory);
        var modelPath = Path.Combine(_modelsDirectory, $"ggml-{_modelType.ToString().ToLowerInvariant()}.bin");

        if (File.Exists(modelPath))
        {
            _logger.Info($"Using cached Whisper model '{modelPath}'.");
            return modelPath;
        }

        _logger.Info($"Whisper model not found locally. Downloading '{_modelType}'...");
        await using var remoteStream = await WhisperGgmlDownloader.Default
            .GetGgmlModelAsync(_modelType, cancellationToken: cancellationToken);
        await using var fileStream = File.Create(modelPath);
        await remoteStream.CopyToAsync(fileStream, cancellationToken);
        _logger.Info("Whisper model downloaded.");

        return modelPath;
    }
}
