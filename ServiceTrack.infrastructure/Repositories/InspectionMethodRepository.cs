using AuthApp.application.Interfaces;
using AuthApp.domain.Entities;
using AuthApp.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthApp.infrastructure.Repositories;

public class InspectionMethodRepository : IInspectionMethodRepository
{
    private readonly ApplicationDbContext _context;

    public InspectionMethodRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<InspectionMethod>> GetAllAsync()
    {
        return await _context.InspectionMethods.ToListAsync();
    }

    public async Task<InspectionMethod?> GetByIdAsync(int id)
    {
        var inspectionMethod = await _context.InspectionMethods.FirstOrDefaultAsync(i => i.Id == id);
        if (inspectionMethod == null)
            return null;
        return inspectionMethod;
    }

    public async Task<IEnumerable<InspectionMethod>> GetByIdsAsync(IEnumerable<int> ids)
    {
        var idList  = ids.ToList();
        return await _context.InspectionMethods.Where(i => idList.Contains(i.Id)).ToListAsync(); 
    }

    public async Task<InspectionMethod?> GetByNameAsync(string code)
    {
        var inspectionMethod = await _context.InspectionMethods.FirstOrDefaultAsync(i => i.Code == code);
        if (inspectionMethod == null)
            return null;
        return inspectionMethod;
    }

    public async Task<InspectionMethod> CreateAsync(InspectionMethod inspectionMethod)
    {
        _context.InspectionMethods.Add(inspectionMethod);
        await _context.SaveChangesAsync();
        return inspectionMethod;
    }

    public async Task<List<InspectionMethod>> CreateBulkAsync(List<InspectionMethod> inspectionMethods)
    {
        _context.InspectionMethods.AddRange(inspectionMethods);
        await _context.SaveChangesAsync();
        return inspectionMethods;
    }

    public async Task<List<InspectionMethod>> UpdateBulkAsync(List<InspectionMethod> inspectionMethods)
    {
        _context.InspectionMethods.UpdateRange(inspectionMethods);
        await _context.SaveChangesAsync();
        return inspectionMethods;
    }

    public async Task<List<InspectionMethod>> DeleteBulkAsync(List<InspectionMethod> inspectionMethods)
    {
        _context.InspectionMethods.RemoveRange(inspectionMethods);
        await _context.SaveChangesAsync();
        return inspectionMethods;
    }
}