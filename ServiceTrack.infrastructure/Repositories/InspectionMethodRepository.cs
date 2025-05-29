using AuthApp.application.Interfaces;
using AuthApp.domain.Entities;
using AuthApp.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthApp.infrastructure.Repositories;

public class InspectionMethodRepository : IInspectionMethodRepository
{
    private readonly ApplicationDbContext _dbContext;

    public InspectionMethodRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<InspectionMethod>> GetAllAsync()
    {
        return await _dbContext.InspectionMethods.ToListAsync();
    }

    public async Task<InspectionMethod?> GetByIdAsync(int id)
    {
        var inspectionMethod = await _dbContext.InspectionMethods.FirstOrDefaultAsync(i => i.Id == id);
        if (inspectionMethod == null)
            return null;
        return inspectionMethod;
    }

    public async Task<InspectionMethod?> GetByNameAsync(string code)
    {
        var inspectionMethod = await _dbContext.InspectionMethods.FirstOrDefaultAsync(i => i.Code == code);
        if (inspectionMethod == null)
            return null;
        return inspectionMethod;
    }

    public async Task<InspectionMethod> CreateAsync(InspectionMethod inspectionMethod)
    {
        _dbContext.InspectionMethods.Add(inspectionMethod);
        await _dbContext.SaveChangesAsync();
        return inspectionMethod;
    }

    public async Task<List<InspectionMethod>> CreateBulkAsync(List<InspectionMethod> inspectionMethods)
    {
        await _dbContext.BulkInsertAsync(inspectionMethods, options =>
        {
            options.AutoMapOutputDirection = false;
        });
        return inspectionMethods;
}

    public async Task<List<InspectionMethod>> UpdateBulkAsync(List<InspectionMethod> inspectionMethods)
    {
        await _dbContext.BulkUpdateAsync(inspectionMethods);
        return inspectionMethods;
    }

    public async Task<List<InspectionMethod>> DeleteBulkAsync(List<InspectionMethod> inspectionMethods)
    {
        await _dbContext.BulkDeleteAsync(inspectionMethods);
        return inspectionMethods;
    }
}