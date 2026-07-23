namespace Backend.Application.Common.Interfaces;

/// <summary>
/// Represents a unit of work for managing database operations.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
