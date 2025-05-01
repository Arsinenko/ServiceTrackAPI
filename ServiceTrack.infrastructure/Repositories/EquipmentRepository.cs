using AuthApp.application.Interfaces;
using AuthApp.domain.Entities;
using AuthApp.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthApp.infrastructure.Repositories;

public class EquipmentRepository : IEquipmentRepository
{
    private readonly ApplicationDbContext _context;

    public EquipmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Equipment?> GetByIdAsync(Guid id)
    {
        return await _context.Equipment.FindAsync(id);
    }

    public async Task<Equipment?> GetByNameAsync(string name)
    {
        return await _context.Equipment.FirstOrDefaultAsync(e => e.Name.ToLower() == name);
    }

    public async Task<IEnumerable<Equipment>> GetAllAsync()
    {
        return await _context.Equipment.ToListAsync();
    }

    public async Task<Guid> CreateAsync(Equipment equipment)
    {
        equipment.CreatedAt = DateTime.UtcNow;
        _context.Equipment.Add(equipment);
        await _context.SaveChangesAsync();
        return equipment.Id;
    }

    public async Task<Guid?> UpdateAsync(Equipment equipment)
    {
        equipment.UpdatedAt = DateTime.UtcNow;
        _context.Equipment.Update(equipment);
        await _context.SaveChangesAsync();
        return equipment.Id;
    }

    public async Task DeleteAsync(Guid id)
    {
        var equipment = await _context.Equipment.FindAsync(id);
        if (equipment != null)
        {
            _context.Equipment.Remove(equipment);
            await _context.SaveChangesAsync();
        }
        
    }
}