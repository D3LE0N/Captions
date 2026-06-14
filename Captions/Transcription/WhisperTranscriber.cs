using System.Text;
using Whisper.net;

namespace Captions.Transcription;

/// <summary>
/// Transcribes audio with Whisper.net. The underlying <see cref="WhisperFactory"/> is created
/// once from the model file and reused for every file, which is the expensive resource to
/// allocate. Language detection is left on automatic.
/// </summary>
public sealed class WhisperTranscriber : ITranscriber, IDisposable
{
    private readonly WhisperFactory _factory;

    public WhisperTranscriber(string modelPath)
    {
        _factory = WhisperFactory.FromPath(modelPath);
    }

    public async Task<string> TranscribeAsync(string wavPath, CancellationToken cancellationToken)
    {
        using var processor = _factory.CreateBuilder()
            .WithLanguage("auto")
            .Build();

        await using var audioStream = File.OpenRead(wavPath);

        var builder = new StringBuilder();
        await foreach (var segment in processor.ProcessAsync(audioStream, cancellationToken))
        {
            builder.Append(segment.Text);
        }

        return builder.ToString().Trim();
    }

    public void Dispose() => _factory.Dispose();
}
