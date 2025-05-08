using AuthApp.application.DTOs;

namespace AuthApp.application.Interfaces;

public interface IJobTypeService
{
    Task<JobTypeDto?> GetByIdAsync(Guid id);
    Task<JobTypeDto?> GetBeyNameAsync(string name);
    Task<IEnumerable<JobTypeDto>>  GetAllAsync();
    Task<JobTypeDto> CreateAsync(CreateJobTypeDto jobTypeDto);
    Task<IEnumerable<JobTypeDto>> CreateBulkAsync(CreateJobTypeBulkDto jobTypeBulkDto);
    Task<JobTypeDto?> UpdateAsync(Guid id,  UpdateJobTypeDto jobTypeDto);
    Task DeleteAsync(Guid id);
}