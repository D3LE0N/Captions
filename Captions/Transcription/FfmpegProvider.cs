using Captions.Logging;
using Xabe.FFmpeg.Downloader;

namespace Captions.Transcription;

/// <summary>
/// Ensures ffmpeg lives in a local tools folder next to the executable, downloading the
/// official build (via Xabe.FFmpeg.Downloader) when it is not already cached. Only the
/// download capability of the package is used; extraction invokes the binary directly.
/// </summary>
public sealed class FfmpegProvider : IFfmpegProvider
{
    private readonly string _toolsDirectory;
    private readonly IAppLogger _logger;

    public FfmpegProvider(string toolsDirectory, IAppLogger logger)
    {
        _toolsDirectory = toolsDirectory;
        _logger = logger;
    }

    public async Task<string> EnsureAvailableAsync(CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(_toolsDirectory);
        var executablePath = Path.Combine(_toolsDirectory, ExecutableName);

        if (File.Exists(executablePath))
        {
            _logger.Info($"Using cached ffmpeg in '{_toolsDirectory}'.");
            return executablePath;
        }

        _logger.Info("ffmpeg not found locally. Downloading the official build...");
        await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official, _toolsDirectory);
        _logger.Info("ffmpeg downloaded.");

        if (!File.Exists(executablePath))
            throw new FileNotFoundException($"ffmpeg was not found after download at '{executablePath}'.");

        return executablePath;
    }

    private static string ExecutableName => OperatingSystem.IsWindows() ? "ffmpeg.exe" : "ffmpeg";
}
