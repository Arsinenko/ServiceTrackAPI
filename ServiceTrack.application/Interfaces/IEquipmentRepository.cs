using AuthApp.application.DTOs;
using AuthApp.domain.Entities;

namespace AuthApp.application.Interfaces;

public interface IEquipmentRepository
{
    Task<Equipment?> GetByIdAsync(Guid id);
    Task<Equipment?> GetByNameAsync(string name);
    Task<IEnumerable<Equipment>> GetAllAsync();
    Task<Guid> CreateAsync(Equipment equipment);
    Task<Guid?> UpdateAsync(Equipment equipment);
    Task<List<Equipment>?> UpdateBulkAsync(IEnumerable<Equipment> equipment);
    Task DeleteAsync(Guid id);
    
    // Component management methods
    Task<Equipment?> AddComponentAsync(Guid equipmentId, Equipment component);
    Task<Equipment?> UpdateComponentAsync(Guid equipmentId, Guid componentId, Equipment component);
    Task<bool> RemoveComponentAsync(Guid equipmentId, Guid componentId);
    Task<Equipment?> GetComponentAsync(Guid equipmentId, Guid componentId);

    Task<List<Guid>> CreateBulkAsync(IEnumerable<Equipment> equipment);
}