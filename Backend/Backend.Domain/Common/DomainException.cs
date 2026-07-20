namespace Backend.Domain.Common;

/// <summary>
/// Exception for domain events errors.
/// </summary>
/// <param name="message">Exception message.</param>
public sealed class DomainException(string message) : Exception(message);
