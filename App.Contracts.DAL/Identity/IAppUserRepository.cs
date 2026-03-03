using App.Domain.Identity;
using Contracts.DAL;

namespace App.Contracts.DAL.Identity;

/// <summary>
/// Repository interface for AppUser aggregate.
/// </summary>
public interface IAppUserRepository : IRepository<AppUser>
{
    /// <summary>
    /// Gets a user by their email address.
    /// </summary>
    /// <param name="email">The email address.</param>
    /// <returns>The user, or null if not found.</returns>
    Task<AppUser?> GetByEmailAsync(string email);
    
    /// <summary>
    /// Gets a user by their username.
    /// </summary>
    /// <param name="userName">The username.</param>
    /// <returns>The user, or null if not found.</returns>
    Task<AppUser?> GetByUserNameAsync(string userName);
}
