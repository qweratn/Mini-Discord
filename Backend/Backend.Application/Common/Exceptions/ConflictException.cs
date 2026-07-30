namespace Backend.Application.Common.Exceptions;

/// <summary>
/// Represents an expected failure caused by a conflict with the current state.
/// </summary>
public sealed class ConflictException : UseCaseException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConflictException"/> class.
    /// </summary>
    /// <param name="code">A stable, machine-readable error code.</param>
    /// <param name="message">A human-readable error message.</param>
    public ConflictException(string code, string message)
        : base(code, message)
    {
    }
}
