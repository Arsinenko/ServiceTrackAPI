using AuthApp.application.DTOs;

namespace AuthApp.application.Interfaces;

public interface IEquipmentInterface
{
    Task<EquipmentDto?> GetByIdAsync(Guid id);
    Task<EquipmentDto?> GetByNameAsync(string name);
    Task<IEnumerable<EquipmentDto>> GetAllAsync();
    Task<EquipmentDto> CreateAsync(CreateEquipmentDto createEquipmentDto);
    Task<EquipmentDto?> UpdateAsync(Guid id, UpdateEquipmentDto updateEquipmentDto);
    Task DeleteAsync(Guid id);

}