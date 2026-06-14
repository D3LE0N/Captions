namespace Captions.Logging;

/// <summary>
/// Minimal logging abstraction so the rest of the app does not depend on <see cref="Console"/>
/// directly, keeping the services testable.
/// </summary>
public interface IAppLogger
{
    void Info(string message);

    void Warn(string message);

    void Error(string message);
}
