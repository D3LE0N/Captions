namespace Captions.Logging;

/// <summary>Writes log messages to the console, routing warnings and errors to stderr.</summary>
public sealed class ConsoleLogger : IAppLogger
{
    public void Info(string message) => Console.WriteLine(message);

    public void Warn(string message) => Console.Error.WriteLine($"warning: {message}");

    public void Error(string message) => Console.Error.WriteLine($"error: {message}");
}
