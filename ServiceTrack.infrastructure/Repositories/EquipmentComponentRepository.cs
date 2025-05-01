using AuthApp.application.Interfaces;
using AuthApp.domain.Entities;
using AuthApp.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthApp.infrastructure.Repositories;

public class EquipmentComponentRepository : IEquipmentComponentRepository
{
    private readonly ApplicationDbContext _context;

    public EquipmentComponentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EquipmentComponent?> GetByIdAsync(Guid id)
    {
        return await _context.EquipmentComponents.FindAsync(id);
    }

    public async Task<IEnumerable<EquipmentComponent>> GetByEquipmentIdAsync(Guid equipmentId)
    {
        return await _context.EquipmentComponents
            .Where(c => c.EquipmentId == equipmentId)
            .ToListAsync();
    }

    public async Task<IEnumerable<EquipmentComponent>> GetAllAsync()
    {
        return await _context.EquipmentComponents.ToListAsync();
    }

    public async Task<Guid> CreateAsync(EquipmentComponent component)
    {
        component.CreatedAt = DateTime.UtcNow;
        component.UpdatedAt = DateTime.UtcNow;
        _context.EquipmentComponents.Add(component);
        await _context.SaveChangesAsync();
        return component.Id;
    }

    public async Task<Guid?> UpdateAsync(EquipmentComponent component)
    {
        component.UpdatedAt = DateTime.UtcNow;
        _context.EquipmentComponents.Update(component);
        await _context.SaveChangesAsync();
        return component.Id;
    }

    public async Task DeleteAsync(Guid id)
    {
        var component = await _context.EquipmentComponents.FindAsync(id);
        if (component != null)
        {
            _context.EquipmentComponents.Remove(component);
            await _context.SaveChangesAsync();
        }
    }
} 