using AuthApp.application.DTOs;
using AuthApp.application.Interfaces;
using AuthApp.domain.Entities;

namespace AuthApp.application.Services;

public class JobTypeService : IJobTypeService
{
    private readonly IJobTypeRepository _jobTypeRepository;

    public JobTypeService(IJobTypeRepository jobTypeRepository)
    {
        _jobTypeRepository = jobTypeRepository;
    }

    public async Task<JobTypeDto?> GetByIdAsync(Guid id)
    {
        var jobType = await _jobTypeRepository.GetByIdAsync(id);
        return jobType != null ? JobTypeDto.FromJobType(jobType) : null;
    }

    public async Task<JobTypeDto?> GetBeyNameAsync(string name)
    {
        var jobType = await _jobTypeRepository.GetByNameAsync(name);
        return jobType != null ? JobTypeDto.FromJobType(jobType) : null;
    }

    public async Task<IEnumerable<JobTypeDto>> GetAllAsync()
    {
        var jobTypes = await _jobTypeRepository.GetAllAsync();
        return jobTypes.Select(JobTypeDto.FromJobType);
    }

    public async Task<JobTypeDto> CreateAsync(CreateJobTypeDto jobTypeDto)
    {
        var jobType = new JobType
        {
            Id = Guid.NewGuid(),
            Name = jobTypeDto.Name,
            Description = jobTypeDto.Description
        };
        await _jobTypeRepository.CreateAsync(jobType);
        return JobTypeDto.FromJobType(jobType);
    }

    public async Task<IEnumerable<JobTypeDto>> CreateBulkAsync(CreateJobTypeBulkDto jobTypeBulkDto)
    {
        var jobTypes = jobTypeBulkDto.JobTypes.Select(dto => new JobType
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description
        }).ToList();
        await _jobTypeRepository.CreateBulkAsync(jobTypes);
        return jobTypes.Select(JobTypeDto.FromJobType);
    }

    public async Task<JobTypeDto?> UpdateAsync(Guid id, UpdateJobTypeDto jobTypeDto)
    {
        var jobType = await _jobTypeRepository.GetByIdAsync(id);
        if (jobType == null)
            return null;
        jobType.Name = jobTypeDto.Name;
        jobType.Description = jobTypeDto.Description;
        
        await _jobTypeRepository.UpdateAsync(jobType);
        return JobTypeDto.FromJobType(jobType);
        
    }

    public async Task DeleteAsync(Guid id)
    {
        await _jobTypeRepository.DeleteAsync(id);
    }
}