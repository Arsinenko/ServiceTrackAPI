using AuthApp.application.DTOs;
using AuthApp.application.Interfaces;
using AuthApp.domain.Entities;

namespace AuthApp.application.Services;

public class EquipmentComponentService : IEquipmentComponentService
{
    private readonly IEquipmentComponentRepository _componentRepository;

    public EquipmentComponentService(IEquipmentComponentRepository componentRepository)
    {
        _componentRepository = componentRepository;
    }

    public async Task<EquipmentComponentDto?> GetByIdAsync(Guid id)
    {
        var component = await _componentRepository.GetByIdAsync(id);
        return component != null ? EquipmentComponentDto.FromEquipmentComponent(component) : null;
    }

    public async Task<IEnumerable<EquipmentComponentDto>> GetByEquipmentIdAsync(Guid equipmentId)
    {
        var components = await _componentRepository.GetByEquipmentIdAsync(equipmentId);
        return components.Select(EquipmentComponentDto.FromEquipmentComponent);
    }

    public async Task<IEnumerable<EquipmentComponentDto>> GetAllAsync()
    {
        var components = await _componentRepository.GetAllAsync();
        return components.Select(EquipmentComponentDto.FromEquipmentComponent);
    }

    public async Task<EquipmentComponentDto> CreateAsync(CreateEquipmentComponentDto createComponentDto)
    {
        var component = new EquipmentComponent
        {
            Id = Guid.NewGuid(),
            Name = createComponentDto.Name,
            Model = createComponentDto.Model,
            SerialNumber = createComponentDto.SerialNumber,
            Manufacturer = createComponentDto.Manufacturer,
            Description = createComponentDto.Description,
            Quantity = createComponentDto.Quantity,
            EquipmentId = createComponentDto.EquipmentId,
            ParentComponentId = createComponentDto.ParentComponentId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _componentRepository.CreateAsync(component);
        return EquipmentComponentDto.FromEquipmentComponent(component);
    }

    public async Task<EquipmentComponentDto> CreateChildComponentAsync(Guid parentId, CreateEquipmentComponentDto createComponentDto)
    {
        // Проверяем существование родительского компонента
        var parentComponent = await _componentRepository.GetByIdAsync(parentId);
        if (parentComponent == null)
        {
            throw new ArgumentException($"Parent component with ID {parentId} not found");
        }

        var component = new EquipmentComponent
        {
            Id = Guid.NewGuid(),
            Name = createComponentDto.Name,
            Model = createComponentDto.Model,
            SerialNumber = createComponentDto.SerialNumber,
            Manufacturer = createComponentDto.Manufacturer,
            Description = createComponentDto.Description,
            Quantity = createComponentDto.Quantity,
            EquipmentId = parentComponent.EquipmentId, // Используем EquipmentId родительского компонента
            ParentComponentId = parentId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _componentRepository.CreateAsync(component);
        return EquipmentComponentDto.FromEquipmentComponent(component);
    }

    public async Task<EquipmentComponentDto?> UpdateAsync(Guid id, UpdateEquipmentComponentDto updateComponentDto)
    {
        var component = await _componentRepository.GetByIdAsync(id);
        if (component == null)
        {
            return null;
        }

        component.Name = updateComponentDto.Name;
        component.Model = updateComponentDto.Model;
        component.SerialNumber = updateComponentDto.SerialNumber;
        component.Manufacturer = updateComponentDto.Manufacturer;
        component.Description = updateComponentDto.Description;
        component.Quantity = updateComponentDto.Quantity;
        component.EquipmentId = updateComponentDto.EquipmentId;
        component.ParentComponentId = updateComponentDto.ParentComponentId;
        component.UpdatedAt = DateTime.UtcNow;

        await _componentRepository.UpdateAsync(component);
        return EquipmentComponentDto.FromEquipmentComponent(component);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _componentRepository.DeleteAsync(id);
    }
} 