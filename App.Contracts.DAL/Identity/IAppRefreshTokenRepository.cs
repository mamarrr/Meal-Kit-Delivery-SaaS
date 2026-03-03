using App.Domain.Identity;
using Contracts.DAL;

namespace App.Contracts.DAL.Identity;

/// <summary>
/// Repository interface for AppRefreshToken aggregate.
/// </summary>
public interface IAppRefreshTokenRepository : IRepository<AppRefreshToken>
{
    /// <summary>
    /// Gets all refresh tokens for a specific user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>A collection of refresh tokens.</returns>
    Task<ICollection<AppRefreshToken>> GetAllByUserIdAsync(Guid userId);
    
    /// <summary>
    /// Gets a refresh token by its token value.
    /// </summary>
    /// <param name="refreshToken">The refresh token value.</param>
    /// <returns>The refresh token, or null if not found.</returns>
    Task<AppRefreshToken?> GetByTokenAsync(string refreshToken);
}
