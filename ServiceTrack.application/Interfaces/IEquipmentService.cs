using AuthApp.application.DTOs;

namespace AuthApp.application.Interfaces;

public interface IEquipmentService
{
    Task<EquipmentDto?> GetByIdAsync(Guid id);
    Task<EquipmentDto?> GetByNameAsync(string name);
    Task<IEnumerable<EquipmentDto>> GetAllAsync();
    Task<EquipmentDto> CreateAsync(CreateEquipmentDto createEquipmentDto);
    Task<IEnumerable<EquipmentDto>> CreateBulkAsync(CreateEquipmentBulkDto createEquipmentBulkDto);
    Task<EquipmentDto?> UpdateAsync(UpdateEquipmentDto updateEquipmentDto);
    Task<List<EquipmentDto>> UpdateBulkAsync(UpdateEquipmentBulkDto updateEquipmentBulkDto);
    Task DeleteAsync(Guid id);
    
    Task<DeleteEquipmentBulkResult> DeleteBulkAsync(IEnumerable<Guid> equipmentIds);

    // Component management methods
    Task<EquipmentDto?> AddComponentAsync(Guid equipmentId, CreateEquipmentDto componentDto);
    Task<bool> RemoveComponentAsync(Guid equipmentId, Guid componentId);
    Task<EquipmentDto?> GetComponentAsync(Guid equipmentId, Guid componentId);
}