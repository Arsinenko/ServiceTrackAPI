using AuthApp.domain.Entities;

namespace AuthApp.application.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(Guid id);
    Task<Role?> GetByNameAsync(string name);
    Task<IEnumerable<Role>> GetAllAsync();
    Task<Guid> CreateAsync(Role role);
    Task<List<Guid>> CreateBulkAsync(IEnumerable<Role> roles);
    Task<Guid> UpdateAsync(Role role);
    Task DeleteAsync(Guid id);
} 