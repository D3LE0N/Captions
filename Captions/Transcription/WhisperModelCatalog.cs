using Whisper.net.Ggml;

namespace Captions.Transcription;

/// <summary>
/// Maps user-facing model names (e.g. "small") to the corresponding <see cref="GgmlType"/>.
/// Keeps the CLI layer free of any dependency on Whisper.net types.
/// </summary>
public static class WhisperModelCatalog
{
    private static readonly IReadOnlyDictionary<string, GgmlType> Models =
        new Dictionary<string, GgmlType>(StringComparer.OrdinalIgnoreCase)
        {
            ["tiny"] = GgmlType.Tiny,
            ["base"] = GgmlType.Base,
            ["small"] = GgmlType.Small,
            ["medium"] = GgmlType.Medium,
            ["large"] = GgmlType.LargeV3,
            ["large-turbo"] = GgmlType.LargeV3Turbo,
        };

    /// <summary>The default model when <c>--model</c> is not provided.</summary>
    public const string DefaultName = "base";

    /// <summary>Supported model names, ordered from fastest/least accurate to slowest/most accurate.</summary>
    public static IReadOnlyList<string> SupportedNames { get; } =
        ["tiny", "base", "small", "medium", "large", "large-turbo"];

    /// <summary>Resolves a model name into its <see cref="GgmlType"/>, or false when unknown.</summary>
    public static bool TryResolve(string name, out GgmlType modelType) =>
        Models.TryGetValue(name, out modelType);
}
