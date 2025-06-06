using AuthApp.domain.Entities;

namespace AuthApp.application.Interfaces;

public interface IJobTypeRepository
{
    Task<JobType?> GetByIdAsync(Guid id);
    Task<IEnumerable<JobType>> GetByIdsAsync(IEnumerable<Guid> ids);
    Task<JobType?> GetByNameAsync(string name);
    Task<IEnumerable<JobType>> GetAllAsync();
    Task<Guid> CreateAsync(JobType jobType);
    Task<List<Guid>> CreateBulkAsync(IEnumerable<JobType> jobTypes);
    Task<Guid> UpdateAsync(JobType jobType);
    Task<List<JobType>?> UpdateBulkAsync(IEnumerable<JobType> jobTypes);
    Task DeleteAsync(Guid id);
    
}