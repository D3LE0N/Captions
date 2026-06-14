namespace Captions.Cli;

/// <summary>
/// Outcome of parsing the command-line arguments: either parsed options, a request to
/// display help, or a validation error. Keeps <see cref="CliParser"/> free of side effects.
/// </summary>
public sealed class CliParseResult
{
    private CliParseResult(CliOptions? options, bool helpRequested, string? error)
    {
        Options = options;
        HelpRequested = helpRequested;
        Error = error;
    }

    /// <summary>Parsed options when parsing succeeded; otherwise null.</summary>
    public CliOptions? Options { get; }

    /// <summary>True when the user asked for <c>--help</c>.</summary>
    public bool HelpRequested { get; }

    /// <summary>Human-readable error when the arguments were invalid; otherwise null.</summary>
    public string? Error { get; }

    public static CliParseResult Success(CliOptions options) => new(options, false, null);

    public static CliParseResult Help() => new(null, true, null);

    public static CliParseResult Failure(string error) => new(null, false, error);
}
