using AuthApp.application.DTOs;
using AuthApp.application.Interfaces;
using AuthApp.application.Services;
using AuthApp.domain.Entities;
using Moq;

namespace ServiceTrack.Tests.Application;

public class EquipmentServiceTests
{
    private readonly Mock<IEquipmentRepository> _equipmentRepositoryMock;
    private readonly EquipmentService _service;

    public EquipmentServiceTests()
    {
        _equipmentRepositoryMock = new Mock<IEquipmentRepository>();
        _service = new EquipmentService(_equipmentRepositoryMock.Object);
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
} 