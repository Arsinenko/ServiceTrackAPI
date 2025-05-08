using AuthApp.application.Interfaces;
using AuthApp.domain.Entities;
using AuthApp.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthApp.infrastructure.Repositories;

public class ServiceRequestRepository : IServiceRequestRepository
{
    private readonly ApplicationDbContext _context;

    public ServiceRequestRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceRequest?> GetByIdAsync(int id)
    {
        return await _context.ServiceRequests
            .Include(sr => sr.JobType)
            .Include(sr => sr.UserServiceRequests)
                .ThenInclude(usr => usr.User)
            .FirstOrDefaultAsync(sr => sr.Id == id);
    }

    public async Task<IEnumerable<ServiceRequest>> GetAllAsync()
    {
        return await _context.ServiceRequests
            .Include(sr => sr.JobType)
            .Include(sr => sr.UserServiceRequests)
                .ThenInclude(usr => usr.User)
            .ToListAsync();
    }

    public async Task<IEnumerable<ServiceRequest>> GetByUserIdAsync(Guid userId)
    {
        return await _context.ServiceRequests
            .Include(sr => sr.JobType)
            .Include(sr => sr.UserServiceRequests)
                .ThenInclude(usr => usr.User)
            .Where(sr => sr.UserServiceRequests.Any(usr => usr.UserId == userId))
            .ToListAsync();
    }

    public async Task<int> CreateAsync(ServiceRequest request)
    {
        request.CreatedAt = DateTime.UtcNow;
        _context.ServiceRequests.Add(request);
        await _context.SaveChangesAsync();
        return request.Id;
    }

    public async Task<List<int>> CreateBulkAsync(IEnumerable<ServiceRequest> requests)
    {
        var requestList = requests.ToList();
        foreach (var request in requestList)
        {
            request.CreatedAt = DateTime.UtcNow;
        }

        await _context.BulkInsertAsync(requestList, options =>
        {
            options.AutoMapOutputDirection = false;
        });
        return requestList.Select(sr => sr.Id).ToList();

    }

    public async Task<int> UpdateAsync(ServiceRequest request)
    {
        request.UpdatedAt = DateTime.UtcNow;
        if (request.IsCompleted && !request.CompletedAt.HasValue)
        {
            request.CompletedAt = DateTime.UtcNow;
        }

        // Update the main entity
        _context.ServiceRequests.Update(request);

        // Update the join table entries
        var existingAssignments = await _context.UserServiceRequests
            .Where(usr => usr.ServiceRequestId == request.Id)
            .ToListAsync();

        // Remove assignments that are no longer present
        var assignmentsToRemove = existingAssignments
            .Where(ea => !request.UserServiceRequests.Any(na => 
                na.UserId == ea.UserId && na.ServiceRequestId == ea.ServiceRequestId))
            .ToList();

        _context.UserServiceRequests.RemoveRange(assignmentsToRemove);

        // Add new assignments
        var newAssignments = request.UserServiceRequests
            .Where(na => !existingAssignments.Any(ea => 
                ea.UserId == na.UserId && ea.ServiceRequestId == na.ServiceRequestId))
            .ToList();

        _context.UserServiceRequests.AddRange(newAssignments);

        await _context.SaveChangesAsync();
        return request.Id;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var request = await _context.ServiceRequests
            .Include(sr => sr.UserServiceRequests)
            .FirstOrDefaultAsync(sr => sr.Id == id);

        if (request == null)
            return false;

        // Remove all user assignments
        _context.UserServiceRequests.RemoveRange(request.UserServiceRequests);
        
        // Remove the service request
        _context.ServiceRequests.Remove(request);
        
        await _context.SaveChangesAsync();
        return true;
    }
} 