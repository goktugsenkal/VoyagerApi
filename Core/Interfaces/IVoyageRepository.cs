using Core.Entities;

namespace Core.Interfaces;

/// <summary>
/// Provides methods to interact with the Voyage database table.
/// </summary>
public interface IVoyageRepository
{
    /// <summary>
    /// Gets all voyages from the database.
    /// </summary>
    /// <returns>A collection of voyages.</returns>
    public Task<ICollection<Voyage>> GetAllVoyages();

    /// <summary>
    /// Gets a voyage by ID from the database.
    /// </summary>
    /// <param name="voyageId">The ID of the voyage to get.</param>
    /// <returns>The voyage if found, otherwise <see langword="null"/>.</returns>
    public Task<Voyage?> GetVoyageById(Guid voyageId);

    /// <summary>
    /// Adds a new voyage to the database.
    /// </summary>
    /// <param name="voyage">The voyage to add.</param>
    Task AddAsync(Voyage voyage);

    /// <summary>
    /// Updates a voyage in the database.
    /// </summary>
    /// <param name="voyage">The voyage to update.</param>
    public Task UpdateVoyage(Voyage voyage);

    /// <summary>
    /// Deletes a voyage from the database.
    /// </summary>
    /// <param name="voyage">The voyage to delete.</param>
    public Task DeleteVoyage(Voyage voyage);

    /// <summary>
    /// Increments the like count of a voyage in the database. To be used with the like service
    /// </summary>
    /// <param name="voyageId">The ID of the voyage to increment the like count for.</param>
    /// <returns><see langword="true"/> if the increment was successful, otherwise <see langword="false"/>.</returns>
    Task<bool> IncrementLikesAsync(Guid voyageId);

    /// <summary>
    /// Decrements the like count of a voyage in the database. To be used with the like service
    /// </summary>
    /// <param name="voyageId">The ID of the voyage to decrement the like count for.</param>
    /// <returns><see langword="true"/> if the decrement was successful, otherwise <see langword="false"/>.</returns>
    Task<bool> DecrementLikesAsync(Guid voyageId);

    /// <summary>
    /// Increments the comment count of a voyage in the database. To be used with the comment service
    /// </summary>
    /// <param name="voyageId">The ID of the voyage to increment the comment count for.</param>
    /// <returns><see langword="true"/> if the increment was successful, otherwise <see langword="false"/>.</returns>
    Task<bool> IncrementCommentsAsync(Guid voyageId);

    /// <summary>
    /// Decrements the comment count of a voyage in the database. To be used with the comment service
    /// </summary>
    /// <param name="voyageId">The ID of the voyage to decrement the comment count for.</param>
    /// <returns><see langword="true"/> if the decrement was successful, otherwise <see langword="false"/>.</returns>
    Task<bool> DecrementCommentsAsync(Guid voyageId);
}