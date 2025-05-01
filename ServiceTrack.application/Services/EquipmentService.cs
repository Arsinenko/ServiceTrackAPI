using AuthApp.application.DTOs;
using AuthApp.application.Interfaces;
using AuthApp.domain.Entities;

namespace AuthApp.application.Services;

public class EquipmentService : IEquipmentService
{
    private readonly IEquipmentRepository _equipmentRepository;


    public EquipmentService(IEquipmentRepository equipmentRepository)
    {
        _equipmentRepository = equipmentRepository;
    }
    public async Task<EquipmentDto?> GetByIdAsync(Guid id)
    {
        var equipment = await _equipmentRepository.GetByIdAsync(id);
        return equipment != null ? EquipmentDto.FromEquipment(equipment) : null;
    }

    public async Task<EquipmentDto?> GetByNameAsync(string name)
    {
        var equipment = await _equipmentRepository.GetByNameAsync(name);
        return equipment != null ? EquipmentDto.FromEquipment(equipment) : null;
    }

    public async Task<IEnumerable<EquipmentDto>> GetAllAsync()
    {
        var equipment = await _equipmentRepository.GetAllAsync();
        return equipment.Select(EquipmentDto.FromEquipment);
    }

    public async Task<EquipmentDto> CreateAsync(CreateEquipmentDto createEquipmentDto)
    {
        var equipment = new Equipment
        {
            Id = Guid.NewGuid(),
            Name = createEquipmentDto.Name,
            Model = createEquipmentDto.Model,
            SerialNumber = createEquipmentDto.SerialNumber,
            Manufacturer = createEquipmentDto.Manufacturer,
            Quantity = createEquipmentDto.Quantity
        };
        await _equipmentRepository.CreateAsync(equipment);
        return EquipmentDto.FromEquipment(equipment);
    }

    public async Task<EquipmentDto?> UpdateAsync(Guid id, UpdateEquipmentDto updateEquipmentDto)
    {
        var equipment = await _equipmentRepository.GetByIdAsync(id);
        if (equipment == null)
        {
            return null;
        }
        equipment.Name = updateEquipmentDto.Name;
        equipment.Model = updateEquipmentDto.Model;
        equipment.SerialNumber = updateEquipmentDto.SerialNumber;
        equipment.Manufacturer = updateEquipmentDto.Manufacturer;
        equipment.Quantity = updateEquipmentDto.Quantity;
        
        await _equipmentRepository.UpdateAsync(equipment);
        return EquipmentDto.FromEquipment(equipment);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _equipmentRepository.DeleteAsync(id); 
    }
}