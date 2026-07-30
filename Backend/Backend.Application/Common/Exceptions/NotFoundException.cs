namespace Backend.Application.Common.Exceptions;

/// <summary>
/// Represents an expected failure when a requested resource does not exist.
/// </summary>
public sealed class NotFoundException : UseCaseException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class.
    /// </summary>
    /// <param name="code">A stable, machine-readable error code.</param>
    /// <param name="message">A human-readable error message.</param>
    public NotFoundException(string code, string message)
        : base(code, message)
    {
    }
}
