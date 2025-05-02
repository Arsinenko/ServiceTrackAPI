using AuthApp.application.DTOs;

namespace AuthApp.application.Interfaces;

public interface IEquipmentComponentService
{
    Task<EquipmentComponentDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<EquipmentComponentDto>> GetByEquipmentIdAsync(Guid equipmentId);
    Task<IEnumerable<EquipmentComponentDto>> GetAllAsync();
    Task<EquipmentComponentDto> CreateAsync(CreateEquipmentComponentDto createComponentDto);
    Task<EquipmentComponentDto> CreateChildComponentAsync(Guid parentId, CreateEquipmentComponentDto createComponentDto);
    Task<EquipmentComponentDto?> UpdateAsync(Guid id, UpdateEquipmentComponentDto updateComponentDto);
    Task DeleteAsync(Guid id);
} 