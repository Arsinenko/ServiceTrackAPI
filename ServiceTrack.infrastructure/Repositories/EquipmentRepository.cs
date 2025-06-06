using AuthApp.application.Interfaces;
using AuthApp.domain.Entities;
using AuthApp.infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AuthApp.infrastructure.Repositories;

public class EquipmentRepository : IEquipmentRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EquipmentRepository> _logger;

    public EquipmentRepository(ApplicationDbContext context, ILogger<EquipmentRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    public async Task<Equipment?> GetByIdAsync(Guid id)
    {
        return await _context.Equipment
            .Include(e => e.Components)
            .Include(e => e.Executor)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Equipment>> GetByIdsAsync(IEnumerable<Guid> ids)
    {
        return await _context.Equipment
            .Include(e => e.Executor)
            .Where(e => ids.Contains(e.Id))
            .ToListAsync();   
    }

    public async Task<Equipment?> GetByNameAsync(string name)
    {
        return await _context.Equipment
            .Include(e => e.Components)
            .Include(e => e.Executor)
            .Include(e => e.SecurityLevel)
            .Include(e => e.EquipmentInspectionMethods)
            .ThenInclude(e => e.InspectionMethod)
            .FirstOrDefaultAsync(e => e.Name.ToLower() == name.ToLower());
    }

    public async Task<IEnumerable<Equipment>> GetAllAsync()
    {
        return await _context.Equipment
            .Include(e => e.Components)
            .Include(e => e.Executor)
            .Include(e => e.SecurityLevel)
            .Include(e => e.EquipmentInspectionMethods)
            .ThenInclude(e => e.InspectionMethod)
            .Include(e => e.Attachments)
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

    public async Task<List<Equipment>?> UpdateBulkAsync(IEnumerable<Equipment> equipment)
    {
        var equipmentList = equipment.ToList();
        foreach (var equipmentEntity in equipmentList)
        {
            equipmentEntity.UpdatedAt = DateTime.UtcNow;
            _context.Equipment.Update(equipmentEntity);
        }
        
        await _context.SaveChangesAsync();
        return equipmentList;
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
        component.ParentId = equipmentId;

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

    public async Task<List<Guid>> CreateBulkAsync(IEnumerable<Equipment> equipment)
    {
        var equipmentList = equipment.ToList();
        try
        {
            _logger.LogInformation("Starting bulk insert of {Count} equipment items", equipmentList.Count);
            
            // Flatten the hierarchy to include all equipment items
            var flattenedEquipment = new List<Equipment>();
            foreach (var item in equipmentList)
            {
                flattenedEquipment.Add(item);
                if (item.Components != null)
                {
                    FlattenEquipmentHierarchy(item.Components, flattenedEquipment);
                }
            }
            
            _logger.LogInformation("Flattened hierarchy contains {Count} total equipment items", flattenedEquipment.Count);
            
            // Log the flattened equipment structure
            foreach (var item in flattenedEquipment)
            {
                LogEquipmentStructure(item);
            }

            foreach (var equipmentEntity in flattenedEquipment)
            {
                equipmentEntity.CreatedAt = DateTime.UtcNow;
                _context.Equipment.Add(equipmentEntity);
            }
            
            _logger.LogInformation("Executing bulk insert with standard EF Core");
            await _context.SaveChangesAsync();
            
            return flattenedEquipment.Select(e => e.Id).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during bulk insert of equipment");
            throw;
        }
    }

    private void FlattenEquipmentHierarchy(IEnumerable<Equipment> components, List<Equipment> flattenedList)
    {
        foreach (var component in components)
        {
            flattenedList.Add(component);
            if (component.Components != null)
            {
                FlattenEquipmentHierarchy(component.Components, flattenedList);
            }
        }
    }

    private void LogEquipmentStructure(Equipment equipment, int depth = 0)
    {
        var indent = new string(' ', depth * 2);
        _logger.LogInformation("{Indent}Equipment: {Name} (ID: {Id}, ParentId: {ParentId})", 
            indent, equipment.Name, equipment.Id, equipment.ParentId);
        
        if (equipment.Components != null)
        {
            foreach (var component in equipment.Components)
            {
                LogEquipmentStructure(component, depth + 1);
            }
        }
    }
}