using System.Diagnostics;

namespace Captions.Transcription;

/// <summary>
/// Extracts audio by invoking the ffmpeg executable directly, producing the 16 kHz mono PCM
/// WAV format that Whisper.net requires. Arguments are passed as a list so paths with spaces
/// need no manual quoting.
/// </summary>
public sealed class FfmpegAudioExtractor : IAudioExtractor
{
    private readonly string _ffmpegPath;

    public FfmpegAudioExtractor(string ffmpegPath)
    {
        _ffmpegPath = ffmpegPath;
    }

    public async Task<string> ExtractToWavAsync(string videoPath, CancellationToken cancellationToken)
    {
        var wavPath = Path.Combine(Path.GetTempPath(), $"captions-{Guid.NewGuid():N}.wav");

        var startInfo = new ProcessStartInfo
        {
            FileName = _ffmpegPath,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
        };

        foreach (var argument in BuildArguments(videoPath, wavPath))
            startInfo.ArgumentList.Add(argument);

        using var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException($"Failed to start ffmpeg at '{_ffmpegPath}'.");

        // Drain stderr so the process does not block on a full pipe buffer.
        var stderrTask = process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);
        var stderr = await stderrTask;

        if (process.ExitCode != 0)
            throw new InvalidOperationException($"ffmpeg exited with code {process.ExitCode}: {Tail(stderr)}");

        return wavPath;
    }

    private static IEnumerable<string> BuildArguments(string videoPath, string wavPath) =>
    [
        "-y",               // overwrite the output if it already exists
        "-i", videoPath,    // input video
        "-vn",              // drop the video stream
        "-ar", "16000",     // 16 kHz sample rate (Whisper requirement)
        "-ac", "1",         // mono
        "-c:a", "pcm_s16le",// signed 16-bit PCM
        "-f", "wav",        // WAV container
        wavPath,
    ];

    private static string Tail(string text, int maxLength = 500) =>
        text.Length <= maxLength ? text : text[^maxLength..];
}
