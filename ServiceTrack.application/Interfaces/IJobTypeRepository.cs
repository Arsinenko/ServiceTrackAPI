using AuthApp.domain.Entities;

namespace AuthApp.application.Interfaces;

public interface IJobTypeRepository
{
    Task<JobType?> GetByIdAsync(Guid id);
    Task<JobType?> GetByNameAsync(string name);
    Task<IEnumerable<JobType>> GetAllAsync();
    Task<Guid> CreateAsync(JobType jobType);
    Task<Guid> UpdateAsync(JobType jobType);
    Task DeleteAsync(Guid id);
    
}