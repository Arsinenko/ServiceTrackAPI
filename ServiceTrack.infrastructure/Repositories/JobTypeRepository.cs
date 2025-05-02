using AuthApp.application.Interfaces;
using AuthApp.domain.Entities;
using AuthApp.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthApp.infrastructure.Repositories;

public class JobTypeRepository :  IJobTypeRepository
{
    private readonly ApplicationDbContext _context;

    public JobTypeRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<JobType?> GetByIdAsync(Guid id)
    {
        return await _context.JobTypes.FindAsync(id);
    }

    public async Task<JobType?> GetByNameAsync(string name)
    {
        return await _context.JobTypes.FirstOrDefaultAsync(j => j.Name == name);
    }

    public async Task<IEnumerable<JobType>> GetAllAsync()
    {
        return await _context.JobTypes.ToListAsync();
    }

    public async Task<Guid> CreateAsync(JobType jobType)
    {
        jobType.CreatedAt = DateTime.UtcNow;
        _context.JobTypes.Add(jobType);
        await _context.SaveChangesAsync();
        return jobType.Id;
    }

    public async Task<Guid> UpdateAsync(JobType jobType)
    {
        jobType.UpdatedAt = DateTime.UtcNow;
        _context.JobTypes.Update(jobType);
        await _context.SaveChangesAsync();
        return jobType.Id;
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}