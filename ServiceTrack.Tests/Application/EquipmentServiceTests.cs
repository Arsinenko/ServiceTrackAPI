using AuthApp.application.DTOs;
using AuthApp.application.Interfaces;
using AuthApp.application.Services;
using AuthApp.domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace ServiceTrack.Tests.Application;

public class EquipmentServiceTests
{
    private readonly Mock<IEquipmentRepository> _equipmentRepositoryMock;
    private readonly Mock<ILogger<EquipmentService>> _loggerMock;
    private readonly EquipmentService _service;

    public EquipmentServiceTests()
    {
        _equipmentRepositoryMock = new Mock<IEquipmentRepository>();
        _loggerMock = new Mock<ILogger<EquipmentService>>();
        _service = new EquipmentService(_equipmentRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEquipmentExists_ReturnsEquipmentDto()
    {
        // Arrange
        var equipmentId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        var updatedAt = DateTime.UtcNow;
        var equipment = new Equipment
        {
            Id = equipmentId,
            Name = "Test Equipment",
            Model = "Test Model",
            SerialNumber = "123456",
            Manufacturer = "Test Manufacturer",
            Quantity = 1,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        _equipmentRepositoryMock
            .Setup(repo => repo.GetByIdAsync(equipmentId))
            .ReturnsAsync(equipment);

        // Act
        var result = await _service.GetByIdAsync(equipmentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(equipmentId, result.Id);
        Assert.Equal(equipment.Name, result.Name);
        Assert.Equal(equipment.Model, result.Model);
        Assert.Equal(equipment.SerialNumber, result.SerialNumber);
        Assert.Equal(equipment.Manufacturer, result.Manufacturer);
        Assert.Equal(equipment.Quantity, result.Quantity);
        Assert.Equal(createdAt.ToLocalTime(), result.CreatedAt);
        Assert.Equal(updatedAt.ToLocalTime(), result.UpdatedAt);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEquipmentDoesNotExist_ReturnsNull()
    {
        // Arrange
        var equipmentId = Guid.NewGuid();
        _equipmentRepositoryMock
            .Setup(repo => repo.GetByIdAsync(equipmentId))
            .ReturnsAsync((Equipment)null);

        // Act
        var result = await _service.GetByIdAsync(equipmentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllEquipment()
    {
        // Arrange
        var equipment = new List<Equipment>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Equipment 1",
                Model = "Model 1",
                SerialNumber = "123456",
                Manufacturer = "Manufacturer 1",
                Quantity = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Equipment 2",
                Model = "Model 2",
                SerialNumber = "789012",
                Manufacturer = "Manufacturer 2",
                Quantity = 2,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _equipmentRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(equipment);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, e => e.Name == "Equipment 1");
        Assert.Contains(result, e => e.Name == "Equipment 2");
    }

    [Fact]
    public async Task CreateAsync_ReturnsCreatedEquipmentDto()
    {
        // Arrange
        var createDto = new CreateEquipmentDto
        {
            Name = "New Equipment",
            Model = "New Model",
            SerialNumber = "123456",
            Manufacturer = "New Manufacturer",
            Quantity = 1
        };

        var equipmentId = Guid.NewGuid();
        _equipmentRepositoryMock
            .Setup(repo => repo.CreateAsync(It.IsAny<Equipment>()))
            .ReturnsAsync(equipmentId);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(createDto.Name, result.Name);
        Assert.Equal(createDto.Model, result.Model);
        Assert.Equal(createDto.SerialNumber, result.SerialNumber);
        Assert.Equal(createDto.Manufacturer, result.Manufacturer);
        Assert.Equal(createDto.Quantity, result.Quantity);
    }

    [Fact]
    public async Task UpdateAsync_WhenEquipmentExists_UpdatesAndReturnsEquipmentDto()
    {
        // Arrange
        var equipmentId = Guid.NewGuid();
        var existingEquipment = new Equipment
        {
            Id = equipmentId,
            Name = "Old Equipment",
            Model = "Old Model",
            SerialNumber = "123456",
            Manufacturer = "Old Manufacturer",
            Quantity = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var updateDto = new UpdateEquipmentDto
        {
            Name = "Updated Equipment",
            Model = "Updated Model",
            SerialNumber = "789012",
            Manufacturer = "Updated Manufacturer",
            Quantity = 2
        };

        _equipmentRepositoryMock
            .Setup(repo => repo.GetByIdAsync(equipmentId))
            .ReturnsAsync(existingEquipment);

        _equipmentRepositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Equipment>()))
            .ReturnsAsync(equipmentId);

        // Act
        var result = await _service.UpdateAsync(equipmentId, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updateDto.Name, result.Name);
        Assert.Equal(updateDto.Model, result.Model);
        Assert.Equal(updateDto.SerialNumber, result.SerialNumber);
        Assert.Equal(updateDto.Manufacturer, result.Manufacturer);
        Assert.Equal(updateDto.Quantity, result.Quantity);
    }

    [Fact]
    public async Task UpdateAsync_WhenEquipmentDoesNotExist_ReturnsNull()
    {
        // Arrange
        var equipmentId = Guid.NewGuid();
        var updateDto = new UpdateEquipmentDto
        {
            Name = "Updated Equipment",
            Model = "Updated Model",
            SerialNumber = "789012",
            Manufacturer = "Updated Manufacturer",
            Quantity = 2
        };

        _equipmentRepositoryMock
            .Setup(repo => repo.GetByIdAsync(equipmentId))
            .ReturnsAsync((Equipment)null);

        // Act
        var result = await _service.UpdateAsync(equipmentId, updateDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenEquipmentExists_ReturnsTrue()
    {
        // Arrange
        var equipmentId = Guid.NewGuid();
        _equipmentRepositoryMock
            .Setup(repo => repo.DeleteAsync(equipmentId))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(equipmentId);

        // Assert
        _equipmentRepositoryMock.Verify(repo => repo.DeleteAsync(equipmentId), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenEquipmentDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var equipmentId = Guid.NewGuid();
        _equipmentRepositoryMock
            .Setup(repo => repo.DeleteAsync(equipmentId))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(equipmentId);

        // Assert
        _equipmentRepositoryMock.Verify(repo => repo.DeleteAsync(equipmentId), Times.Once);
    }

    [Fact]
    public async Task AddComponentAsync_WhenEquipmentExists_AddsAndReturnsUpdatedEquipmentDto()
    {
        // Arrange
        var equipmentId = Guid.NewGuid();
        var componentId = Guid.NewGuid();
        var existingEquipment = new Equipment
        {
            Id = equipmentId,
            Name = "Test Equipment",
            Description = "Test Description",
            SerialNumber = "SN123",
            Model = "Test Model",
            Manufacturer = "Test Manufacturer",
            Quantity = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Components = new List<Equipment>()
        };

        var componentDto = new CreateEquipmentDto
        {
            Name = "Test Component",
            Description = "Test Component Description",
            SerialNumber = "CSN123",
            Model = "Test Component Model",
            Manufacturer = "Test Component Manufacturer"
        };

        var component = new Equipment
        {
            Id = componentId,
            Name = componentDto.Name,
            Description = componentDto.Description,
            SerialNumber = componentDto.SerialNumber,
            Model = componentDto.Model,
            Manufacturer = componentDto.Manufacturer,
            Quantity = componentDto.Quantity,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        existingEquipment.Components.Add(component);

        _equipmentRepositoryMock
            .Setup(repo => repo.AddComponentAsync(equipmentId, It.IsAny<Equipment>()))
            .ReturnsAsync(existingEquipment);

        // Act
        var result = await _service.AddComponentAsync(equipmentId, componentDto);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Components);
        Assert.Single(result.Components);
        var resultComponent = result.Components.First();
        Assert.Equal(componentDto.Name, resultComponent.Name);
        Assert.Equal(componentDto.Description, resultComponent.Description);
        Assert.Equal(componentDto.SerialNumber, resultComponent.SerialNumber);
        Assert.Equal(componentDto.Model, resultComponent.Model);
        Assert.Equal(componentDto.Manufacturer, resultComponent.Manufacturer);
    }

    [Fact]
    public async Task UpdateComponentAsync_WhenComponentExists_UpdatesAndReturnsEquipmentDto()
    {
        // Arrange
        var equipmentId = Guid.NewGuid();
        var componentId = Guid.NewGuid();
        var existingEquipment = new Equipment
        {
            Id = equipmentId,
            Name = "Test Equipment",
            Description = "Test Description",
            SerialNumber = "SN123",
            Model = "Test Model",
            Manufacturer = "Test Manufacturer",
            Quantity = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Components = new List<Equipment>
            {
                new()
                {
                    Id = componentId,
                    Name = "Old Component",
                    Description = "Old Description",
                    SerialNumber = "OldCSN",
                    Model = "Old Model",
                    Manufacturer = "Old Manufacturer"
                }
            }
        };

        var updateDto = new UpdateEquipmentDto
        {
            Name = "New Component",
            Description = "New Description",
            SerialNumber = "NewCSN",
            Model = "New Model",
            Manufacturer = "New Manufacturer"
        };

        var updatedEquipment = new Equipment
        {
            Id = equipmentId,
            Name = "Test Equipment",
            Description = "Test Description",
            SerialNumber = "SN123",
            Model = "Test Model",
            Manufacturer = "Test Manufacturer",
            Quantity = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Components = new List<Equipment>
            {
                new()
                {
                    Id = componentId,
                    Name = updateDto.Name,
                    Description = updateDto.Description,
                    SerialNumber = updateDto.SerialNumber,
                    Model = updateDto.Model,
                    Manufacturer = updateDto.Manufacturer
                }
            }
        };

        _equipmentRepositoryMock
            .Setup(repo => repo.UpdateComponentAsync(equipmentId, componentId, It.IsAny<Equipment>()))
            .ReturnsAsync(updatedEquipment);

        // Act
        var result = await _service.UpdateComponentAsync(equipmentId, componentId, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Components);
        Assert.Single(result.Components);
        var component = result.Components.First();
        Assert.Equal(updateDto.Name, component.Name);
        Assert.Equal(updateDto.Description, component.Description);
        Assert.Equal(updateDto.SerialNumber, component.SerialNumber);
        Assert.Equal(updateDto.Model, component.Model);
        Assert.Equal(updateDto.Manufacturer, component.Manufacturer);
    }

    [Fact]
    public async Task RemoveComponentAsync_WhenComponentExists_ReturnsTrue()
    {
        // Arrange
        var equipmentId = Guid.NewGuid();
        var componentId = Guid.NewGuid();
        var existingEquipment = new Equipment
        {
            Id = equipmentId,
            Name = "Test Equipment",
            Description = "Test Description",
            SerialNumber = "SN123",
            Model = "Test Model",
            Manufacturer = "Test Manufacturer",
            Quantity = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Components = new List<Equipment>
            {
                new()
                {
                    Id = componentId,
                    Name = "Test Component",
                    Description = "Test Description",
                    SerialNumber = "CSN123",
                    Model = "Test Model",
                    Manufacturer = "Test Manufacturer"
                }
            }
        };

        _equipmentRepositoryMock
            .Setup(repo => repo.RemoveComponentAsync(equipmentId, componentId))
            .ReturnsAsync(true);

        // Act
        var result = await _service.RemoveComponentAsync(equipmentId, componentId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task GetComponentAsync_WhenComponentExists_ReturnsComponentDto()
    {
        // Arrange
        var equipmentId = Guid.NewGuid();
        var componentId = Guid.NewGuid();
        var component = new Equipment
        {
            Id = componentId,
            Name = "Test Component",
            Description = "Test Description",
            SerialNumber = "CSN123",
            Model = "Test Model",
            Manufacturer = "Test Manufacturer",
            Quantity = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _equipmentRepositoryMock
            .Setup(repo => repo.GetComponentAsync(equipmentId, componentId))
            .ReturnsAsync(component);

        // Act
        var result = await _service.GetComponentAsync(equipmentId, componentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(componentId, result.Id);
        Assert.Equal("Test Component", result.Name);
        Assert.Equal("Test Description", result.Description);
        Assert.Equal("CSN123", result.SerialNumber);
        Assert.Equal("Test Model", result.Model);
        Assert.Equal("Test Manufacturer", result.Manufacturer);
    }

    [Fact]
    public async Task CreateBulkAsync_ValidData_CreatesAndReturnsEquipmentDtos()
    {
        // Arrange
        var createBulkDto = new CreateEquipmentBulkDto
        {
            Equipment = new List<CreateEquipmentDto>
            {
                new() { Name = "Equipment 1", Model = "Model 1", SerialNumber = "SN1", Manufacturer = "Manufacturer 1", Quantity = 1 },
                new() { Name = "Equipment 2", Model = "Model 2", SerialNumber = "SN2", Manufacturer = "Manufacturer 2", Quantity = 2 }
            }
        };

        _equipmentRepositoryMock
            .Setup(repo => repo.CreateBulkAsync(It.IsAny<IEnumerable<Equipment>>()))
            .ReturnsAsync(new List<Guid> { Guid.NewGuid(), Guid.NewGuid() });

        // Act
        var result = await _service.CreateBulkAsync(createBulkDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, e => e.Name == "Equipment 1" && e.Model == "Model 1");
        Assert.Contains(result, e => e.Name == "Equipment 2" && e.Model == "Model 2");
    }

    [Fact]
    public async Task UpdateBulkAsync_ValidData_UpdatesAndReturnsEquipmentDtos()
    {
        // Arrange
        var equipmentIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var existingEquipment = new List<Equipment>
        {
            new()
            {
                Id = equipmentIds[0],
                Name = "Original Equipment 1",
                Model = "Original Model 1",
                SerialNumber = "OSN1",
                Manufacturer = "Original Manufacturer 1",
                Quantity = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = equipmentIds[1],
                Name = "Original Equipment 2",
                Model = "Original Model 2",
                SerialNumber = "OSN2",
                Manufacturer = "Original Manufacturer 2",
                Quantity = 2,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        var updateBulkDto = new UpdateEquipmentBulkDto
        {
            Equipment = new List<UpdateEquipmentBulkItemDto>
            {
                new() { Id = equipmentIds[0], Name = "Updated Equipment 1", Model = "Updated Model 1", SerialNumber = "USN1", Manufacturer = "Updated Manufacturer 1", Quantity = 1 },
                new() { Id = equipmentIds[1], Name = "Updated Equipment 2", Model = "Updated Model 2", SerialNumber = "USN2", Manufacturer = "Updated Manufacturer 2", Quantity = 2 }
            }
        };

        _equipmentRepositoryMock
            .Setup(repo => repo.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
            .ReturnsAsync(existingEquipment);

        _equipmentRepositoryMock
            .Setup(repo => repo.UpdateBulkAsync(It.IsAny<IEnumerable<Equipment>>()))
            .ReturnsAsync(existingEquipment);

        // Act
        var result = await _service.UpdateBulkAsync(updateBulkDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.UpdatedEquipment.Count);
        Assert.Empty(result.FailedEquipmentIds);
        Assert.Empty(result.FailureReasons);
        Assert.Contains(result.UpdatedEquipment, e => e.Name == "Updated Equipment 1" && e.Model == "Updated Model 1");
        Assert.Contains(result.UpdatedEquipment, e => e.Name == "Updated Equipment 2" && e.Model == "Updated Model 2");
    }

    [Fact]
    public async Task UpdateBulkAsync_WhenSomeEquipmentDoesNotExist_UpdatesExistingAndReportsFailures()
    {
        // Arrange
        var existingId = Guid.NewGuid();
        var nonExistentId = Guid.NewGuid();
        var existingEquipment = new Equipment
        {
            Id = existingId,
            Name = "Original Equipment",
            Model = "Original Model",
            SerialNumber = "OSN",
            Manufacturer = "Original Manufacturer",
            Quantity = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var updateBulkDto = new UpdateEquipmentBulkDto
        {
            Equipment = new List<UpdateEquipmentBulkItemDto>
            {
                new() { Id = existingId, Name = "Updated Equipment", Model = "Updated Model", SerialNumber = "USN", Manufacturer = "Updated Manufacturer", Quantity = 1 },
                new() { Id = nonExistentId, Name = "Non-existent Equipment", Model = "Non-existent Model", SerialNumber = "NSN", Manufacturer = "Non-existent Manufacturer", Quantity = 1 }
            }
        };

        _equipmentRepositoryMock
            .Setup(repo => repo.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
            .ReturnsAsync(new List<Equipment> { existingEquipment });

        _equipmentRepositoryMock
            .Setup(repo => repo.UpdateBulkAsync(It.IsAny<IEnumerable<Equipment>>()))
            .ReturnsAsync(new List<Equipment> { existingEquipment });

        // Act
        var result = await _service.UpdateBulkAsync(updateBulkDto);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.UpdatedEquipment);
        Assert.Single(result.FailedEquipmentIds);
        Assert.Single(result.FailureReasons);
        Assert.Contains(result.UpdatedEquipment, e => e.Id == existingId && e.Name == "Updated Equipment");
        Assert.Contains(nonExistentId, result.FailedEquipmentIds);
        Assert.Contains($"Equipment with ID {nonExistentId} not found", result.FailureReasons);
    }

    [Fact]
    public async Task UpdateBulkAsync_WhenUpdateOperationFails_ReportsFailure()
    {
        // Arrange
        var equipmentId = Guid.NewGuid();
        var existingEquipment = new Equipment
        {
            Id = equipmentId,
            Name = "Original Equipment",
            Model = "Original Model",
            SerialNumber = "OSN",
            Manufacturer = "Original Manufacturer",
            Quantity = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var updateBulkDto = new UpdateEquipmentBulkDto
        {
            Equipment = new List<UpdateEquipmentBulkItemDto>
            {
                new() { Id = equipmentId, Name = "Updated Equipment", Model = "Updated Model", SerialNumber = "USN", Manufacturer = "Updated Manufacturer", Quantity = 1 }
            }
        };

        _equipmentRepositoryMock
            .Setup(repo => repo.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
            .ReturnsAsync(new List<Equipment> { existingEquipment });

        _equipmentRepositoryMock
            .Setup(repo => repo.UpdateBulkAsync(It.IsAny<IEnumerable<Equipment>>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _service.UpdateBulkAsync(updateBulkDto);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.UpdatedEquipment);
        Assert.Single(result.FailedEquipmentIds);
        Assert.Single(result.FailureReasons);
        Assert.Contains(equipmentId, result.FailedEquipmentIds);
        var failureReason = result.FailureReasons.First();
        Assert.Contains("Failed to update equipment with ID", failureReason);
        Assert.Contains("Database error", failureReason);
    }

    [Fact]
    public async Task UpdateBulkAsync_WhenEmptyListProvided_ReturnsEmptyResult()
    {
        // Arrange
        var updateBulkDto = new UpdateEquipmentBulkDto
        {
            Equipment = new List<UpdateEquipmentBulkItemDto>()
        };

        // Act
        var result = await _service.UpdateBulkAsync(updateBulkDto);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.UpdatedEquipment);
        Assert.Empty(result.FailedEquipmentIds);
        Assert.Empty(result.FailureReasons);
        _equipmentRepositoryMock.Verify(repo => repo.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()), Times.Never);
        _equipmentRepositoryMock.Verify(repo => repo.UpdateBulkAsync(It.IsAny<IEnumerable<Equipment>>()), Times.Never);
    }

    [Fact]
    public async Task CreateBulkAsync_WithComponents_CreatesAndReturnsEquipmentWithComponents()
    {
        // Arrange
        var createBulkDto = new CreateEquipmentBulkDto
        {
            Equipment = new List<CreateEquipmentDto>
            {
                new()
                {
                    Name = "Main Equipment",
                    Model = "Model A",
                    SerialNumber = "SN001",
                    Manufacturer = "Manufacturer X",
                    Quantity = 1,
                    Description = "Main equipment description",
                    Components = new List<CreateEquipmentDto>
                    {
                        new()
                        {
                            Name = "Component 1",
                            Model = "Model B",
                            SerialNumber = "SN002",
                            Manufacturer = "Manufacturer Y",
                            Quantity = 2,
                            Description = "Component 1 description",
                            Components = new List<CreateEquipmentDto>
                            {
                                new()
                                {
                                    Name = "Sub-component 1.1",
                                    Model = "Model C",
                                    SerialNumber = "SN003",
                                    Manufacturer = "Manufacturer Z",
                                    Quantity = 1,
                                    Description = "Sub-component description"
                                }
                            }
                        }
                    }
                }
            }
        };

        _equipmentRepositoryMock
            .Setup(repo => repo.CreateBulkAsync(It.IsAny<IEnumerable<Equipment>>()))
            .ReturnsAsync(new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() });

        // Act
        var result = await _service.CreateBulkAsync(createBulkDto);

        // Assert
        Assert.NotNull(result);
        var equipmentList = result.ToList();
        Assert.Single(equipmentList);
        
        var mainEquipment = equipmentList[0];
        Assert.Equal("Main Equipment", mainEquipment.Name);
        Assert.Equal("Model A", mainEquipment.Model);
        Assert.Equal("SN001", mainEquipment.SerialNumber);
        Assert.Equal("Manufacturer X", mainEquipment.Manufacturer);
        Assert.Equal(1, mainEquipment.Quantity);
        Assert.Equal("Main equipment description", mainEquipment.Description);
        
        Assert.NotNull(mainEquipment.Components);
        var components = mainEquipment.Components.ToList();
        Assert.Single(components);
        
        var component = components[0];
        Assert.Equal("Component 1", component.Name);
        Assert.Equal("Model B", component.Model);
        Assert.Equal("SN002", component.SerialNumber);
        Assert.Equal("Manufacturer Y", component.Manufacturer);
        Assert.Equal(2, component.Quantity);
        Assert.Equal("Component 1 description", component.Description);
        
        Assert.NotNull(component.Components);
        var subComponents = component.Components.ToList();
        Assert.Single(subComponents);
        
        var subComponent = subComponents[0];
        Assert.Equal("Sub-component 1.1", subComponent.Name);
        Assert.Equal("Model C", subComponent.Model);
        Assert.Equal("SN003", subComponent.SerialNumber);
        Assert.Equal("Manufacturer Z", subComponent.Manufacturer);
        Assert.Equal(1, subComponent.Quantity);
        Assert.Equal("Sub-component description", subComponent.Description);
    }

    [Fact]
    public async Task DeleteBulkAsync_WhenAllEquipmentExists_DeletesAllAndReturnsSuccess()
    {
        // Arrange
        var equipmentIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var equipment = new List<Equipment>
        {
            new()
            {
                Id = equipmentIds[0],
                Name = "Equipment 1",
                Model = "Model 1",
                SerialNumber = "SN1",
                Manufacturer = "Manufacturer 1",
                Quantity = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = equipmentIds[1],
                Name = "Equipment 2",
                Model = "Model 2",
                SerialNumber = "SN2",
                Manufacturer = "Manufacturer 2",
                Quantity = 2,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _equipmentRepositoryMock
            .Setup(repo => repo.GetByIdAsync(equipmentIds[0]))
            .ReturnsAsync(equipment[0]);
        _equipmentRepositoryMock
            .Setup(repo => repo.GetByIdAsync(equipmentIds[1]))
            .ReturnsAsync(equipment[1]);
        _equipmentRepositoryMock
            .Setup(repo => repo.DeleteAsync(It.IsAny<Guid>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteBulkAsync(equipmentIds);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.DeletedEquipment.Count);
        Assert.Empty(result.FailedEquipmentIds);
        Assert.Empty(result.FailureReasons);
        Assert.Contains(result.DeletedEquipment, e => e.Id == equipmentIds[0] && e.Name == "Equipment 1");
        Assert.Contains(result.DeletedEquipment, e => e.Id == equipmentIds[1] && e.Name == "Equipment 2");
        _equipmentRepositoryMock.Verify(repo => repo.DeleteAsync(equipmentIds[0]), Times.Once);
        _equipmentRepositoryMock.Verify(repo => repo.DeleteAsync(equipmentIds[1]), Times.Once);
    }

    [Fact]
    public async Task DeleteBulkAsync_WhenSomeEquipmentDoesNotExist_DeletesExistingAndReportsFailures()
    {
        // Arrange
        var existingId = Guid.NewGuid();
        var nonExistentId = Guid.NewGuid();
        var equipmentIds = new List<Guid> { existingId, nonExistentId };
        var equipment = new Equipment
        {
            Id = existingId,
            Name = "Existing Equipment",
            Model = "Model 1",
            SerialNumber = "SN1",
            Manufacturer = "Manufacturer 1",
            Quantity = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _equipmentRepositoryMock
            .Setup(repo => repo.GetByIdAsync(existingId))
            .ReturnsAsync(equipment);
        _equipmentRepositoryMock
            .Setup(repo => repo.GetByIdAsync(nonExistentId))
            .ReturnsAsync((Equipment)null);
        _equipmentRepositoryMock
            .Setup(repo => repo.DeleteAsync(existingId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteBulkAsync(equipmentIds);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.DeletedEquipment);
        Assert.Single(result.FailedEquipmentIds);
        Assert.Single(result.FailureReasons);
        Assert.Contains(result.DeletedEquipment, e => e.Id == existingId && e.Name == "Existing Equipment");
        Assert.Contains(nonExistentId, result.FailedEquipmentIds);
        Assert.Contains($"Equipment with ID {nonExistentId} not found", result.FailureReasons);
        _equipmentRepositoryMock.Verify(repo => repo.DeleteAsync(existingId), Times.Once);
        _equipmentRepositoryMock.Verify(repo => repo.DeleteAsync(nonExistentId), Times.Never);
    }

    [Fact]
    public async Task DeleteBulkAsync_WhenDeleteOperationFails_ReportsFailure()
    {
        // Arrange
        var equipmentId = Guid.NewGuid();
        var equipment = new Equipment
        {
            Id = equipmentId,
            Name = "Equipment",
            Model = "Model 1",
            SerialNumber = "SN1",
            Manufacturer = "Manufacturer 1",
            Quantity = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _equipmentRepositoryMock
            .Setup(repo => repo.GetByIdAsync(equipmentId))
            .ReturnsAsync(equipment);
        _equipmentRepositoryMock
            .Setup(repo => repo.DeleteAsync(equipmentId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _service.DeleteBulkAsync(new List<Guid> { equipmentId });

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.DeletedEquipment);
        Assert.Single(result.FailedEquipmentIds);
        Assert.Single(result.FailureReasons);
        Assert.Contains(equipmentId, result.FailedEquipmentIds);
        var failureReason = result.FailureReasons.First();
        Assert.Contains("Failed to delete equipment with ID", failureReason);
        Assert.Contains("Database error", failureReason);
    }

    [Fact]
    public async Task DeleteBulkAsync_WhenEmptyListProvided_ReturnsEmptyResult()
    {
        // Arrange
        var equipmentIds = new List<Guid>();

        // Act
        var result = await _service.DeleteBulkAsync(equipmentIds);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.DeletedEquipment);
        Assert.Empty(result.FailedEquipmentIds);
        Assert.Empty(result.FailureReasons);
        _equipmentRepositoryMock.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _equipmentRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }
} 