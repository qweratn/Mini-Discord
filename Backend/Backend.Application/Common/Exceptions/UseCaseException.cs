namespace Backend.Application.Common.Exceptions;

/// <summary>
/// Base exception for an expected application use-case failure.
/// </summary>
public abstract class UseCaseException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UseCaseException"/> class.
    /// </summary>
    /// <param name="code">A stable, machine-readable error code.</param>
    /// <param name="message">A human-readable error message.</param>
    protected UseCaseException(string code, string message)
        : base(message)
    {
        Code = code;
    }

    /// <summary>
    /// Gets a stable, machine-readable error code.
    /// </summary>
    public string Code { get; }
}
