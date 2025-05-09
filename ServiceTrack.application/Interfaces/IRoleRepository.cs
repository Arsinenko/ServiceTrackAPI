using AuthApp.domain.Entities;

namespace AuthApp.application.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(Guid id);
    Task<Role?> GetByNameAsync(string name);
    Task<IEnumerable<Role>> GetAllAsync();
    Task<Guid> CreateAsync(Role role);
    Task<List<Guid>> CreateBulkAsync(IEnumerable<Role> roles);
    Task<Role> UpdateAsync(Role role);
    Task<List<Role>> UpdateBulkAsync(IEnumerable<Role> roles);
    Task DeleteAsync(Guid id);
} 