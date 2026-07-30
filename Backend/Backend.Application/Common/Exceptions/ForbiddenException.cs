namespace Backend.Application.Common.Exceptions;

/// <summary>
/// Represents an expected failure caused by insufficient permissions.
/// </summary>
public sealed class ForbiddenException : UseCaseException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenException"/> class.
    /// </summary>
    /// <param name="code">A stable, machine-readable error code.</param>
    /// <param name="message">A human-readable error message.</param>
    public ForbiddenException(string code, string message)
        : base(code, message)
    {
    }
}
