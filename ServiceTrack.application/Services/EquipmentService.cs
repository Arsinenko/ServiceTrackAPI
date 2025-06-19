using AuthApp.application.DTOs;
using AuthApp.application.Exceptions;
using AuthApp.application.Interfaces;
using AuthApp.domain.Entities;
using Microsoft.Extensions.Logging;

namespace AuthApp.application.Services;

public class EquipmentService : IEquipmentService
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IInspectionMethodRepository _methodRepository;
    private readonly ILogger<EquipmentService> _logger;

    public EquipmentService(IEquipmentRepository equipmentRepository, IInspectionMethodRepository methodRepository, ILogger<EquipmentService> logger)
    {
        _equipmentRepository = equipmentRepository;
        _methodRepository = methodRepository;
        _logger = logger;
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
            Category = createEquipmentDto.Category,
            Quantity = createEquipmentDto.Quantity,
            ExecutorId = createEquipmentDto.ExecutorId,
            SecurityLevelId = createEquipmentDto.SecurityLevelId,
            SZZ = createEquipmentDto.SZZ,
            Description = createEquipmentDto.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            EquipmentInspectionMethods = new List<EquipmentInspectionMethod>(),
            Attachments = new List<EquipmentAttachment>(),
            ParentId = createEquipmentDto.ParentId == Guid.Empty ? null : createEquipmentDto.ParentId
        };
        foreach (var method in createEquipmentDto.Methods)
        {
            var inspectionMethod =  await _methodRepository.GetByIdAsync(method.InspectionMethodId);
            if (inspectionMethod == null)
            {
                throw new InspectionMethodNotFoundException("Inspection method not found");
            }
            equipment.EquipmentInspectionMethods.Add(new  EquipmentInspectionMethod
            {
                EquipmentId = equipment.Id,
                InspectionMethodId = inspectionMethod.Id
            });
        }
        await _equipmentRepository.CreateAsync(equipment);
        return EquipmentDto.FromEquipment(equipment);
    }

    private async Task<Equipment> CreateEquipmentWithComponentsAsync(CreateEquipmentDto dto, Guid? parentId = null)
    {
        var equipment = new Equipment
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Model = dto.Model,
            SerialNumber = dto.SerialNumber,
            Manufacturer = dto.Manufacturer,
            Category = dto.Category,
            Quantity = dto.Quantity,
            ExecutorId = dto.ExecutorId,
            SecurityLevelId = dto.SecurityLevelId,
            SZZ = dto.SZZ,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            EquipmentInspectionMethods = new List<EquipmentInspectionMethod>(),
            Attachments = new List<EquipmentAttachment>(),
            ParentId = parentId
        };
        foreach (var method in dto.Methods)
        {
            var inspectionMethod =  await _methodRepository.GetByIdAsync(method.InspectionMethodId);
            if (inspectionMethod == null)
            {
                throw new InspectionMethodNotFoundException("Inspection method not found");
            }
            equipment.EquipmentInspectionMethods.Add(new  EquipmentInspectionMethod
            {
                EquipmentId = equipment.Id,
                InspectionMethodId = inspectionMethod.Id
            });
        }

        // Create components recursively if they exist
        if (dto.Components != null && dto.Components.Any())
        {
            equipment.Components = new List<Equipment>();
            foreach (var componentDto in dto.Components)
            {
                var component = await CreateEquipmentWithComponentsAsync(componentDto, equipment.Id);
                equipment.Components.Add(component);
            }
        }

        return equipment;
    }

    public async Task<IEnumerable<EquipmentDto>> CreateBulkAsync(CreateEquipmentBulkDto createEquipmentBulkDto)
    {
        var equipmentList = new List<Equipment>();
        
        try
        {
            _logger.LogInformation("Starting bulk equipment creation with {Count} main items", createEquipmentBulkDto.Equipment.Count);
            
            foreach (var dto in createEquipmentBulkDto.Equipment)
            {
                _logger.LogInformation("Creating equipment: {Name} with {ComponentCount} components", 
                    dto.Name, dto.Components?.Count ?? 0);
                
                var equipment = await CreateEquipmentWithComponentsAsync(dto);
                equipmentList.Add(equipment);
                
                // Log the created equipment structure
                LogEquipmentStructure(equipment);
            }

            _logger.LogInformation("Attempting to save {Count} equipment items to database", equipmentList.Count);
            var ids = await _equipmentRepository.CreateBulkAsync(equipmentList);
            _logger.LogInformation("Successfully saved equipment with IDs: {Ids}", string.Join(", ", ids));
            
            return equipmentList.Select(EquipmentDto.FromEquipment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during bulk equipment creation");
            throw;
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

    private async Task<Equipment> CreateComponentAsync(CreateEquipmentDto componentDto, Guid parentId)
    {
        var component = new Equipment
        {
            Id = Guid.NewGuid(),
            Name = componentDto.Name,
            Model = componentDto.Model,
            SerialNumber = componentDto.SerialNumber,
            Manufacturer = componentDto.Manufacturer,
            Category = componentDto.Category,
            Quantity = componentDto.Quantity,
            ExecutorId = componentDto.ExecutorId,
            SecurityLevelId = componentDto.SecurityLevelId,
            SZZ = componentDto.SZZ,
            Description = componentDto.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            EquipmentInspectionMethods = new List<EquipmentInspectionMethod>(),
            Attachments = new List<EquipmentAttachment>(),
            ParentId = parentId
        };
        foreach (var method in componentDto.Methods)
        {
            var inspectionMethod = await _methodRepository.GetByIdAsync(method.InspectionMethodId);
            if (inspectionMethod == null)
            {
                throw new InspectionMethodNotFoundException($"Inspection method {method.InspectionMethodId} not found");
            }
            component.EquipmentInspectionMethods.Add(new EquipmentInspectionMethod
            {
                EquipmentId = component.Id,
                InspectionMethodId = inspectionMethod.Id
            });
        }
        // Сохраняем компонент в базе
        await _equipmentRepository.CreateAsync(component);
        return component;
    }
    
    public async Task<EquipmentDto?> UpdateAsync(UpdateEquipmentDto updateEquipmentDto)
    {
        var equipment = await _equipmentRepository.GetByIdAsync(updateEquipmentDto.Id);
        if (equipment == null)
        {
            return null;
        }

        // Список свойств, которые не нужно обновлять
        var excludedProperties = new[] 
        { 
            nameof(UpdateEquipmentDto.Id),
            nameof(UpdateEquipmentDto.Methods) 
        };

        // Получаем все свойства DTO, кроме исключенных
        var dtoProperties = typeof(UpdateEquipmentDto).GetProperties()
            .Where(p => !excludedProperties.Contains(p.Name))
            .ToList();

        foreach (var dtoProperty in dtoProperties)
        {
            var value = dtoProperty.GetValue(updateEquipmentDto);
            if (value != null)
            {
                // Находим соответствующее свойство в сущности
                var entityProperty = equipment.GetType().GetProperty(dtoProperty.Name);
                if (entityProperty != null && entityProperty.CanWrite)
                {
                    entityProperty.SetValue(equipment, value);
                }
            }
        }

        if (updateEquipmentDto.Methods != null)
        {
            equipment.EquipmentInspectionMethods = new List<EquipmentInspectionMethod>();
            foreach (var method in updateEquipmentDto.Methods)
            {
                var inspectionMethod = await _methodRepository.GetByIdAsync(method.InspectionMethodId);
                if (inspectionMethod == null)
                {
                    throw new InspectionMethodNotFoundException($"Inspection method {method.InspectionMethodId} not found");
                }
                equipment.EquipmentInspectionMethods.Add(new EquipmentInspectionMethod
                {
                    EquipmentId = equipment.Id,
                    InspectionMethodId = inspectionMethod.Id
                });
            }
        }

        equipment.UpdatedAt = DateTime.UtcNow;

        await _equipmentRepository.UpdateAsync(equipment);
        return EquipmentDto.FromEquipment(equipment);
    }

    public async Task<List<EquipmentDto>> UpdateBulkAsync(UpdateEquipmentBulkDto updateEquipmentBulkDto)
{
    if (updateEquipmentBulkDto.Equipments == null || !updateEquipmentBulkDto.Equipments.Any())
    {
        return new List<EquipmentDto>();
    }

    // 1. Собираем все Id из DTO
    var equipmentIds = updateEquipmentBulkDto.Equipments.Select(e => e.Id).ToList();

    // 2. Получаем всё оборудование одним запросом
    var existingEquipments = await _equipmentRepository.GetByIdsAsync(equipmentIds);
    var equipmentDict = existingEquipments.ToDictionary(e => e.Id);

    // 3. Собираем и проверяем методы осмотра
    var methodIds = updateEquipmentBulkDto.Equipments
        .Where(e => e.Methods != null)
        .SelectMany(e => e.Methods)
        .Select(m => m.InspectionMethodId)
        .Distinct()
        .ToList();

    var methods = await _methodRepository.GetByIdsAsync(methodIds);
    var methodDict = methods.ToDictionary(m => m.Id);

    var missingMethodIds = methodIds.Where(id => !methodDict.ContainsKey(id)).ToList();
    if (missingMethodIds.Count > 0)
    {
        throw new InspectionMethodNotFoundException($"Inspection methods {string.Join(", ", missingMethodIds)} not found.");
    }

    // 4. Обновляем каждую сущность
    var updatedEquipments = new List<Equipment>();

    foreach (var dto in updateEquipmentBulkDto.Equipments)
    {
        if (!equipmentDict.TryGetValue(dto.Id, out var equipment))
        {
            continue; // Пропускаем, если оборудование не найдено
        }

        // Обновляем свойства, кроме Id и Methods
        var excludedProperties = new[]
        {
            nameof(UpdateEquipmentDto.Id),
            nameof(UpdateEquipmentDto.Methods)
        };

        var dtoProperties = typeof(UpdateEquipmentDto).GetProperties()
            .Where(p => !excludedProperties.Contains(p.Name))
            .ToList();

        foreach (var property in dtoProperties)
        {
            var value = property.GetValue(dto);
            if (value != null)
            {
                var entityProperty = equipment.GetType().GetProperty(property.Name);
                if (entityProperty != null && entityProperty.CanWrite)
                {
                    entityProperty.SetValue(equipment, value);
                }
            }
        }

        // Обновляем методы осмотра
        if (dto.Methods != null)
        {
            equipment.EquipmentInspectionMethods.Clear();
            foreach (var methodDto in dto.Methods)
            {
                if (methodDict.TryGetValue(methodDto.InspectionMethodId, out var method))
                {
                    equipment.EquipmentInspectionMethods.Add(new EquipmentInspectionMethod
                    {
                        EquipmentId = equipment.Id,
                        InspectionMethodId = method.Id
                    });
                }
            }
        }

        equipment.UpdatedAt = DateTime.UtcNow;
        updatedEquipments.Add(equipment);
    }

    // 5. Массовое обновление в репозитории
    await _equipmentRepository.UpdateBulkAsync(updatedEquipments);

    // 6. Возвращаем обновлённые DTO
    return updatedEquipments.Select(EquipmentDto.FromEquipment).ToList();
}


    public async Task DeleteAsync(Guid id)
    {
        await _equipmentRepository.DeleteAsync(id); 
    }

    public async Task<DeleteEquipmentBulkResult> DeleteBulkAsync(IEnumerable<Guid> equipmentIds)
    {
        var deletedEquipment = new List<EquipmentDto>();
        var failedEquipmentIds = new List<Guid>();
        var failureReasons = new List<string>();

        foreach (var id in equipmentIds)
        {
            try
            {
                var equipment = await _equipmentRepository.GetByIdAsync(id);
                if (equipment == null)
                {
                    failedEquipmentIds.Add(id);
                    failureReasons.Add($"Equipment with ID {id} not found");
                    continue;
                }

                await _equipmentRepository.DeleteAsync(id);
                deletedEquipment.Add(EquipmentDto.FromEquipment(equipment));
            }
            catch (Exception ex)
            {
                failedEquipmentIds.Add(id);
                failureReasons.Add($"Failed to delete equipment with ID {id}: {ex.Message}");
            }
        }

        return new DeleteEquipmentBulkResult
        {
            DeletedEquipment = deletedEquipment,
            FailedEquipmentIds = failedEquipmentIds,
            FailureReasons = failureReasons
        };
    }

    public async Task<EquipmentDto?> AddComponentAsync(Guid equipmentId, CreateEquipmentDto componentDto)
    {
        var component = await CreateComponentAsync(componentDto, equipmentId);
        var equipment = await _equipmentRepository.AddComponentAsync(equipmentId, component);
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