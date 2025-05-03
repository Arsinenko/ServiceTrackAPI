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
        return await _context.Equipment
            .Include(e => e.Components)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Equipment?> GetByNameAsync(string name)
    {
        return await _context.Equipment
            .Include(e => e.Components)
            .FirstOrDefaultAsync(e => e.Name.ToLower() == name.ToLower());
    }

    public async Task<IEnumerable<Equipment>> GetAllAsync()
    {
        return await _context.Equipment
            .Include(e => e.Components)
            .ToListAsync();
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

    public async Task<Equipment?> AddComponentAsync(Guid equipmentId, Equipment component)
    {
        var equipment = await _context.Equipment
            .Include(e => e.Components)
            .FirstOrDefaultAsync(e => e.Id == equipmentId);

        if (equipment == null)
            return null;

        component.CreatedAt = DateTime.UtcNow;
        component.UpdatedAt = DateTime.UtcNow;
        component.ParentId = equipment;

        if (equipment.Components == null)
            equipment.Components = new List<Equipment>();

        equipment.Components.Add(component);
        await _context.SaveChangesAsync();

        return equipment;
    }

    public async Task<Equipment?> UpdateComponentAsync(Guid equipmentId, Guid componentId, Equipment updatedComponent)
    {
        var equipment = await _context.Equipment
            .Include(e => e.Components)
            .FirstOrDefaultAsync(e => e.Id == equipmentId);

        if (equipment?.Components == null)
            return null;

        var component = equipment.Components.FirstOrDefault(c => c.Id == componentId);
        if (component == null)
            return null;

        // Update component properties
        component.Name = updatedComponent.Name;
        component.Model = updatedComponent.Model;
        component.SerialNumber = updatedComponent.SerialNumber;
        component.Manufacturer = updatedComponent.Manufacturer;
        component.Quantity = updatedComponent.Quantity;
        component.Description = updatedComponent.Description;
        component.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return equipment;
    }

    public async Task<bool> RemoveComponentAsync(Guid equipmentId, Guid componentId)
    {
        var equipment = await _context.Equipment
            .Include(e => e.Components)
            .FirstOrDefaultAsync(e => e.Id == equipmentId);

        if (equipment?.Components == null)
            return false;

        var component = equipment.Components.FirstOrDefault(c => c.Id == componentId);
        if (component == null)
            return false;

        equipment.Components.Remove(component);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Equipment?> GetComponentAsync(Guid equipmentId, Guid componentId)
    {
        var equipment = await _context.Equipment
            .Include(e => e.Components)
            .FirstOrDefaultAsync(e => e.Id == equipmentId);

        return equipment?.Components?.FirstOrDefault(c => c.Id == componentId);
    }
}