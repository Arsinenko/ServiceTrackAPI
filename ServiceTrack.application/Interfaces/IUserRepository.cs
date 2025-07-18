using AuthApp.domain.Entities;

namespace AuthApp.application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<IEnumerable<User>> GetByIdsAsync(IEnumerable<Guid> ids);
    Task<User?> GetByEmailAsync(string email);
    Task<Guid> CreateAsync(User user);
    Task<List<User>> CreateBulkAsync(IEnumerable<User> users);
    Task<Guid> UpdateAsync(User user);
    Task<List<User>> UpdateBulkAsync(IEnumerable<User> users);
    Task<IEnumerable<User>> GetAllAsync();
    Task<bool> SoftDeleteAsync(Guid id);
}