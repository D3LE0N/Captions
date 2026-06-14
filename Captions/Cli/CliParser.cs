using Captions.Transcription;

namespace Captions.Cli;

/// <summary>
/// Parses raw command-line arguments into a <see cref="CliParseResult"/>.
/// Supports both <c>--key value</c> and <c>--key=value</c> forms.
/// </summary>
public sealed class CliParser
{
    public CliParseResult Parse(string[] args)
    {
        string? inputDirectory = null;
        string? outputDirectory = null;
        string? modelName = null;
        var writeEach = false;
        var writeMain = true;

        for (var i = 0; i < args.Length; i++)
        {
            var (name, inlineValue) = SplitArgument(args[i]);

            switch (name)
            {
                case "--help":
                case "-h":
                    return CliParseResult.Help();

                case "--dir":
                    if (!TryReadValue(args, ref i, inlineValue, out var dir))
                        return CliParseResult.Failure("Option '--dir' requires a folder path.");
                    inputDirectory = dir;
                    break;

                case "--out-dir":
                    if (!TryReadValue(args, ref i, inlineValue, out var outDir))
                        return CliParseResult.Failure("Option '--out-dir' requires a folder path.");
                    outputDirectory = outDir;
                    break;

                case "--model":
                    if (!TryReadValue(args, ref i, inlineValue, out var model))
                        return CliParseResult.Failure("Option '--model' requires a model name.");
                    modelName = model;
                    break;

                case "--out-each":
                    writeEach = true;
                    break;

                case "--no-main":
                    writeMain = false;
                    break;

                default:
                    return CliParseResult.Failure($"Unknown option '{args[i]}'. Use --help to see usage.");
            }
        }

        if (string.IsNullOrWhiteSpace(inputDirectory))
            return CliParseResult.Failure("Option '--dir' is required. Use --help to see usage.");

        if (!writeMain && !writeEach)
            return CliParseResult.Failure(
                "Nothing to write: '--no-main' disables the combined file and '--out-each' was not set.");

        modelName ??= WhisperModelCatalog.DefaultName;
        if (!WhisperModelCatalog.TryResolve(modelName, out _))
            return CliParseResult.Failure(
                $"Unknown model '{modelName}'. Supported models: {string.Join(", ", WhisperModelCatalog.SupportedNames)}.");

        var options = new CliOptions
        {
            InputDirectory = Path.GetFullPath(inputDirectory),
            OutputDirectory = Path.GetFullPath(
                string.IsNullOrWhiteSpace(outputDirectory) ? Directory.GetCurrentDirectory() : outputDirectory),
            ModelName = modelName,
            WriteEachTranscript = writeEach,
            WriteMainFile = writeMain,
        };

        return CliParseResult.Success(options);
    }

    /// <summary>Splits "--key=value" into ("--key", "value"); plain flags yield a null value.</summary>
    private static (string Name, string? InlineValue) SplitArgument(string arg)
    {
        var separator = arg.IndexOf('=');
        return separator < 0
            ? (arg, null)
            : (arg[..separator], arg[(separator + 1)..]);
    }

    /// <summary>
    /// Resolves an option value either from the inline "=value" form or the next token,
    /// advancing the loop index when the next token is consumed.
    /// </summary>
    private static bool TryReadValue(string[] args, ref int index, string? inlineValue, out string value)
    {
        if (inlineValue is not null)
        {
            value = inlineValue;
            return value.Length > 0;
        }

        if (index + 1 < args.Length && !args[index + 1].StartsWith('-'))
        {
            value = args[++index];
            return true;
        }

        value = string.Empty;
        return false;
    }
}
