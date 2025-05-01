using AuthApp.domain.Entities;

namespace AuthApp.application.Interfaces;

public interface IEquipmentComponentRepository
{
    Task<EquipmentComponent?> GetByIdAsync(Guid id);
    Task<IEnumerable<EquipmentComponent>> GetByEquipmentIdAsync(Guid equipmentId);
    Task<IEnumerable<EquipmentComponent>> GetAllAsync();
    Task<Guid> CreateAsync(EquipmentComponent component);
    Task<Guid?> UpdateAsync(EquipmentComponent component);
    Task DeleteAsync(Guid id);
} 