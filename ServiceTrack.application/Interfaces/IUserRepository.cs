using AuthApp.domain.Entities;

namespace AuthApp.application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<Guid> CreateAsync(User user);
    Task<Guid> UpdateAsync(User user);
    Task<IEnumerable<User>> GetAllAsync();
    Task<bool> SoftDeleteAsync(Guid id);
}