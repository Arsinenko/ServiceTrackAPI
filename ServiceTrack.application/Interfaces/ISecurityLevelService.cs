using AuthApp.application.DTOs;

namespace ServiceTrack.application.Interfaces;

public interface ISecurityLevelService
{
    Task<SecurityLevelDto?> GetByIdAsync(int id);
    Task<IEnumerable<SecurityLevelDto>> GetAllAsync();
    Task<SecurityLevelDto> CreateAsync(CreateSecurityLevelDto createDto);
    Task<SecurityLevelDto> UpdateAsync(int id, UpdateSecurityLevelDto updateDto);
    Task DeleteAsync(int id);
    Task<IEnumerable<SecurityLevelDto>> BulkCreateAsync(BulkCreateSecurityLevelDto bulkCreateDto);
    Task<IEnumerable<SecurityLevelDto>> BulkUpdateAsync(BulkUpdateSecurityLevelDto bulkUpdateDto);
} 