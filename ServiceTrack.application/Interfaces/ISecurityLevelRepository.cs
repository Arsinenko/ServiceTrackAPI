using AuthApp.domain.Entities;

namespace ServiceTrack.application.Interfaces;

public interface ISecurityLevelRepository
{
    Task<SecurityLevel?> GetByIdAsync(int id);
    Task<IEnumerable<SecurityLevel>> GetAllAsync();
    Task<SecurityLevel> CreateAsync(SecurityLevel securityLevel);
    Task<SecurityLevel> UpdateAsync(SecurityLevel securityLevel);
    Task DeleteAsync(int id);
    Task<bool> ExistsByCodeAsync(string code, int? excludeId = null);
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
} 