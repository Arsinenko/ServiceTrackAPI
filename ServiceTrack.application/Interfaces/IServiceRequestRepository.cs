using AuthApp.domain.Entities;

namespace AuthApp.application.Interfaces;

public interface IServiceRequestRepository
{
    Task<ServiceRequest?> GetByIdAsync(int id);
    Task<IEnumerable<ServiceRequest>> GetAllAsync();
    Task<IEnumerable<ServiceRequest>> GetByUserIdAsync(Guid userId);
    Task<int> CreateAsync(ServiceRequest request);
    Task<List<int>> CreateBulkAsync(IEnumerable<ServiceRequest> requests);
    Task<int> UpdateAsync(ServiceRequest request);
    Task<bool> DeleteAsync(int id);
} 