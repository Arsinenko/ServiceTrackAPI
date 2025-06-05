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
            Attachments = new List<EquipmentAttachment>()
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
            Attachments = new List<EquipmentAttachment>()
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
        equipment.Category = updateEquipmentDto.Category;
        equipment.Quantity = updateEquipmentDto.Quantity;
        equipment.ExecutorId = updateEquipmentDto.ExecutorId;
        equipment.SecurityLevelId = updateEquipmentDto.SecurityLevelId;
        equipment.SZZ = updateEquipmentDto.SZZ;
        equipment.Description = updateEquipmentDto.Description;
        equipment.UpdatedAt = DateTime.UtcNow;
        
        equipment.EquipmentInspectionMethods.Clear();
        foreach (var method in updateEquipmentDto.Methods)
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
        
        await _equipmentRepository.UpdateAsync(equipment);
        return EquipmentDto.FromEquipment(equipment);
    }

    public async Task<UpdateEquipmentBulkResult> UpdateBulkAsync(UpdateEquipmentBulkDto updateEquipmentBulkDto)
    {
        if (!updateEquipmentBulkDto.Equipment.Any())
        {
            return new UpdateEquipmentBulkResult
            {
                UpdatedEquipment = new List<EquipmentDto>(),
                FailedEquipmentIds = new List<Guid>(),
                FailureReasons = new List<string>()
            };
        }

        var updatedEquipment = new List<EquipmentDto>();
        var failedEquipmentIds = new List<Guid>();
        var failureReasons = new List<string>();

        // Get all existing equipment in one query
        var equipmentIds = updateEquipmentBulkDto.Equipment.Select(e => e.Id).ToList();
        var existingEquipment = (await _equipmentRepository.GetByIdsAsync(equipmentIds)).ToDictionary(e => e.Id);

        var equipmentToUpdate = new List<Equipment>();

        foreach (var item in updateEquipmentBulkDto.Equipment)
        {
            try
            {
                if (!existingEquipment.TryGetValue(item.Id, out var existing))
                {
                    failedEquipmentIds.Add(item.Id);
                    failureReasons.Add($"Equipment with ID {item.Id} not found");
                    continue;
                }

                // Update only the fields that are provided in the DTO
                existing.Name = item.Name;
                existing.Model = item.Model;
                existing.SerialNumber = item.SerialNumber;
                existing.Manufacturer = item.Manufacturer;
                existing.Quantity = item.Quantity;
                existing.ExecutorId = item.ExecutorId;
                existing.SZZ = item.SZZ;
                existing.Description = item.Description;
                existing.UpdatedAt = DateTime.UtcNow;
                existing.SecurityLevelId = item.SecurityLevelId;
                existing.EquipmentInspectionMethods.Clear();
                
                foreach (var method in item.Methods)
                {
                    var inspectionMethod =  await _methodRepository.GetByIdAsync(method.InspectionMethodId);
                    if (inspectionMethod == null)
                    {
                        throw new InspectionMethodNotFoundException("Inspection method not found");
                    }
                    existing.EquipmentInspectionMethods.Add(new EquipmentInspectionMethod
                    {
                        EquipmentId = existing.Id,
                        InspectionMethodId = inspectionMethod.Id
                    });
                }
                
                equipmentToUpdate.Add(existing);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error preparing equipment {Id} for update", item.Id);
                failedEquipmentIds.Add(item.Id);
                failureReasons.Add($"Failed to prepare equipment with ID {item.Id} for update: {ex.Message}");
            }
        }

        if (equipmentToUpdate.Any())
        {
            try
            {
                var result = await _equipmentRepository.UpdateBulkAsync(equipmentToUpdate);
                if (result != null)
                {
                    updatedEquipment.AddRange(result.Select(EquipmentDto.FromEquipment));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk update of equipment");
                // Add all equipment IDs that were being updated to the failed list
                failedEquipmentIds.AddRange(equipmentToUpdate.Select(e => e.Id));
                failureReasons.AddRange(equipmentToUpdate.Select(e => 
                    $"Failed to update equipment with ID {e.Id}: {ex.Message}"));
            }
        }

        return new UpdateEquipmentBulkResult
        {
            UpdatedEquipment = updatedEquipment,
            FailedEquipmentIds = failedEquipmentIds,
            FailureReasons = failureReasons
        };
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
        //TODO Fix constructor
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
            SZZ = componentDto.SZZ,
            Description = componentDto.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            EquipmentInspectionMethods = new List<EquipmentInspectionMethod>(),
            Attachments = new List<EquipmentAttachment>()
        };

        var equipment = await _equipmentRepository.AddComponentAsync(equipmentId, component);
        return equipment != null ? EquipmentDto.FromEquipment(equipment) : null;
    }

    public async Task<EquipmentDto?> UpdateComponentAsync(Guid equipmentId, Guid componentId, UpdateEquipmentDto componentDto)
    {
        //TODO Fix constructor
        var updatedComponent = new Equipment
        {
            Name = componentDto.Name,
            Model = componentDto.Model,
            SerialNumber = componentDto.SerialNumber,
            Manufacturer = componentDto.Manufacturer,
            Category = componentDto.Category,
            Quantity = componentDto.Quantity,
            ExecutorId = componentDto.ExecutorId,
            SZZ = componentDto.SZZ,
            Description = componentDto.Description,
            UpdatedAt = DateTime.UtcNow,
            EquipmentInspectionMethods = new List<EquipmentInspectionMethod>()
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