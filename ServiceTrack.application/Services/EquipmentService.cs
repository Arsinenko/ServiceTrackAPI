using AuthApp.application.DTOs;
using AuthApp.application.Interfaces;
using AuthApp.domain.Entities;

namespace AuthApp.application.Services;

public class EquipmentService : IEquipmentInterface
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

    public Task<EquipmentDto?> UpdateAsync(Guid id, UpdateEquipmentDto updateEquipmentDto)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}