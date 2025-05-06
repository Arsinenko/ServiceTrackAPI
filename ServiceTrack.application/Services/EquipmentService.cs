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
            Quantity = createEquipmentDto.Quantity,
            Description = createEquipmentDto.Description
        };
        await _equipmentRepository.CreateAsync(equipment);
        return EquipmentDto.FromEquipment(equipment);
    }

    public async Task<IEnumerable<EquipmentDto>> CreateBulkAsync(CreateEquipmentBulkDto createEquipmentBulkDto)
    {
        var equipmentList = createEquipmentBulkDto.Equipment.Select(dto => new Equipment
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Model = dto.Model,
            SerialNumber = dto.SerialNumber,
            Manufacturer = dto.Manufacturer,
            Quantity = dto.Quantity,
            Description = dto.Description
        }).ToList();

        await _equipmentRepository.CreateBulkAsync(equipmentList);
        return equipmentList.Select(EquipmentDto.FromEquipment);
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
        equipment.Description = updateEquipmentDto.Description;
        
        await _equipmentRepository.UpdateAsync(equipment);
        return EquipmentDto.FromEquipment(equipment);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _equipmentRepository.DeleteAsync(id); 
    }

    public async Task<EquipmentDto?> AddComponentAsync(Guid equipmentId, CreateEquipmentDto componentDto)
    {
        var component = new Equipment
        {
            Id = Guid.NewGuid(),
            Name = componentDto.Name,
            Model = componentDto.Model,
            SerialNumber = componentDto.SerialNumber,
            Manufacturer = componentDto.Manufacturer,
            Quantity = componentDto.Quantity,
            Description = componentDto.Description
        };

        var equipment = await _equipmentRepository.AddComponentAsync(equipmentId, component);
        return equipment != null ? EquipmentDto.FromEquipment(equipment) : null;
    }

    public async Task<EquipmentDto?> UpdateComponentAsync(Guid equipmentId, Guid componentId, UpdateEquipmentDto componentDto)
    {
        var updatedComponent = new Equipment
        {
            Name = componentDto.Name,
            Model = componentDto.Model,
            SerialNumber = componentDto.SerialNumber,
            Manufacturer = componentDto.Manufacturer,
            Quantity = componentDto.Quantity,
            Description = componentDto.Description
        };

        var equipment = await _equipmentRepository.UpdateComponentAsync(equipmentId, componentId, updatedComponent);
        return equipment != null ? EquipmentDto.FromEquipment(equipment) : null;
    }

    public async Task<bool> RemoveComponentAsync(Guid equipmentId, Guid componentId)
    {
        return await _equipmentRepository.RemoveComponentAsync(equipmentId, componentId);
    }

    public async Task<EquipmentDto?> GetComponentAsync(Guid equipmentId, Guid componentId)
    {
        var component = await _equipmentRepository.GetComponentAsync(equipmentId, componentId);
        return component != null ? EquipmentDto.FromEquipment(component) : null;
    }
}