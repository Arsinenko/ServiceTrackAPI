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
            .Include(sr => sr.ServiceRequestEquipments)
                .ThenInclude(sre => sre.Equipment)
                    .ThenInclude(e => e.Components)
            .FirstOrDefaultAsync(sr => sr.Id == id);
    }

    public async Task<IEnumerable<ServiceRequest>> GetAllAsync()
    {
        return await _context.ServiceRequests
            .Include(sr => sr.JobType)
            .Include(sr => sr.UserServiceRequests)
                .ThenInclude(usr => usr.User)
            .Include(sr => sr.ServiceRequestEquipments)
                .ThenInclude(sre => sre.Equipment)
                    .ThenInclude(e => e.Components)
            .ToListAsync();
    }

    public async Task<IEnumerable<ServiceRequest>> GetByUserIdAsync(Guid userId)
    {
        return await _context.ServiceRequests
            .Include(sr => sr.JobType)
            .Include(sr => sr.UserServiceRequests)
                .ThenInclude(usr => usr.User)
            .Include(sr => sr.ServiceRequestEquipments)
                .ThenInclude(sre => sre.Equipment)
                    .ThenInclude(e => e.Components)
            .Where(sr => sr.UserServiceRequests.Any(usr => usr.UserId == userId))
            .ToListAsync();
    }

    public async Task<int> CreateAsync(ServiceRequest request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            request.CreatedAt = DateTime.UtcNow;
            
            // Add the main entity first
            _context.ServiceRequests.Add(request);
            await _context.SaveChangesAsync(); // This will generate the Id

            // Now that we have the Id, we can properly set up the relationships
            if (request.UserServiceRequests != null)
            {
                // Get existing assignments for this request
                var existingAssignments = await _context.UserServiceRequests
                    .Where(usr => usr.ServiceRequestId == request.Id)
                    .ToListAsync();

                foreach (var userRequest in request.UserServiceRequests)
                {
                    // Skip if this assignment already exists
                    if (existingAssignments.Any(ea => 
                        ea.UserId == userRequest.UserId && 
                        ea.ServiceRequestId == request.Id))
                    {
                        continue;
                    }

                    userRequest.ServiceRequestId = request.Id;
                    _context.UserServiceRequests.Add(userRequest);
                }
            }

            if (request.ServiceRequestEquipments != null)
            {
                // Get existing equipment assignments for this request
                var existingEquipment = await _context.ServiceRequestEquipments
                    .Where(sre => sre.ServiceRequestId == request.Id)
                    .ToListAsync();

                foreach (var equipment in request.ServiceRequestEquipments)
                {
                    // Skip if this equipment is already assigned
                    if (existingEquipment.Any(ee => 
                        ee.EquipmentId == equipment.EquipmentId && 
                        ee.ServiceRequestId == request.Id))
                    {
                        continue;
                    }

                    equipment.ServiceRequestId = request.Id;
                    _context.ServiceRequestEquipments.Add(equipment);
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return request.Id;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<int>> CreateBulkAsync(IEnumerable<ServiceRequest> requests)
    {
        var requestList = requests.ToList();
        var createdIds = new List<int>();

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            foreach (var request in requestList)
            {
                request.CreatedAt = DateTime.UtcNow;
                
                // Add the main entity
                _context.ServiceRequests.Add(request);
                await _context.SaveChangesAsync(); // This will generate the Id
                createdIds.Add(request.Id);

                // Now that we have the Id, we can properly set up the relationships
                if (request.UserServiceRequests != null)
                {
                    // Get existing assignments for this request
                    var existingAssignments = await _context.UserServiceRequests
                        .Where(usr => usr.ServiceRequestId == request.Id)
                        .ToListAsync();

                    foreach (var userRequest in request.UserServiceRequests)
                    {
                        // Skip if this assignment already exists
                        if (existingAssignments.Any(ea => 
                            ea.UserId == userRequest.UserId && 
                            ea.ServiceRequestId == request.Id))
                        {
                            continue;
                        }

                        userRequest.ServiceRequestId = request.Id;
                        _context.UserServiceRequests.Add(userRequest);
                    }
                }

                if (request.ServiceRequestEquipments != null)
                {
                    // Get existing equipment assignments for this request
                    var existingEquipment = await _context.ServiceRequestEquipments
                        .Where(sre => sre.ServiceRequestId == request.Id)
                        .ToListAsync();

                    foreach (var equipment in request.ServiceRequestEquipments)
                    {
                        // Skip if this equipment is already assigned
                        if (existingEquipment.Any(ee => 
                            ee.EquipmentId == equipment.EquipmentId && 
                            ee.ServiceRequestId == request.Id))
                        {
                            continue;
                        }

                        equipment.ServiceRequestId = request.Id;
                        _context.ServiceRequestEquipments.Add(equipment);
                    }
                }

                await _context.SaveChangesAsync();
            }

            await transaction.CommitAsync();
            return createdIds;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
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