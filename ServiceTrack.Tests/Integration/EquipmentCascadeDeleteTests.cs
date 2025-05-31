using AuthApp.application.DTOs;
using AuthApp.application.Interfaces;
using AuthApp.application.Services;
using AuthApp.domain.Entities;
using AuthApp.infrastructure.Data;
using AuthApp.infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceTrack.application.Interfaces;
using Xunit;

namespace ServiceTrack.Tests.Integration;

public class EquipmentCascadeDeleteTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IEquipmentRepository> _equipmentRepositoryMock;
    private readonly Mock<ILogger<EquipmentService>> _loggerMock;
    private readonly Mock<IInspectionMethodRepository> _inspectionMethodRepositoryMock;
    private readonly Mock<ISecurityLevelRepository> _securityLevelRepositoryMock;
    private readonly EquipmentService _service;
    private readonly EquipmentRepository _repository;
    private readonly ILogger<EquipmentRepository> _repositoryLogger;
    private readonly ILogger<EquipmentService> _serviceLogger;

    public EquipmentCascadeDeleteTests()
    {
        // Создаем тестовую базу данных в памяти
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"EquipmentTestDb_{Guid.NewGuid()}")
            .Options;

        _context = new ApplicationDbContext(options);
        _repositoryLogger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<EquipmentRepository>();
        _serviceLogger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<EquipmentService>();
        _repository = new EquipmentRepository(_context, _repositoryLogger);
        _equipmentRepositoryMock = new Mock<IEquipmentRepository>();
        _loggerMock = new Mock<ILogger<EquipmentService>>();
        _inspectionMethodRepositoryMock = new Mock<IInspectionMethodRepository>();
        _securityLevelRepositoryMock = new Mock<ISecurityLevelRepository>();
        _service = new EquipmentService(
            _equipmentRepositoryMock.Object,
            _loggerMock.Object,
            _inspectionMethodRepositoryMock.Object,
            _securityLevelRepositoryMock.Object);
    }

    [Fact]
    public async Task DeleteEquipment_WithComponents_DeletesAllComponents()
    {
        // Arrange
        var equipmentId = Guid.NewGuid();
        var componentId = Guid.NewGuid();
        var equipment = new Equipment
        {
            Id = equipmentId,
            Name = "Test Equipment",
            Model = "Test Model",
            SerialNumber = "123456",
            Manufacturer = "Test Manufacturer",
            Category = 1,
            Quantity = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            EquipmentSecurityLevels = new List<EquipmentSecurityLevel>(),
            EquipmentInspectionMethods = new List<EquipmentInspectionMethod>(),
            Components = new List<Equipment>
            {
                new()
                {
                    Id = componentId,
                    Name = "Test Component",
                    Model = "Test Component Model",
                    SerialNumber = "789012",
                    Manufacturer = "Test Component Manufacturer",
                    Category = 1,
                    Quantity = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    EquipmentSecurityLevels = new List<EquipmentSecurityLevel>(),
                    EquipmentInspectionMethods = new List<EquipmentInspectionMethod>()
                }
            }
        };

        _equipmentRepositoryMock
            .Setup(repo => repo.GetByIdAsync(equipmentId))
            .ReturnsAsync(equipment);

        // Act
        await _service.DeleteAsync(equipmentId);

        // Assert
        _equipmentRepositoryMock.Verify(repo => repo.DeleteAsync(equipmentId), Times.Once);
    }

    [Fact]
    public async Task DeleteEquipment_WithoutComponents_DeletesOnlyMainEquipment()
    {
        // Arrange
        var equipment = new Equipment
        {
            Id = Guid.NewGuid(),
            Name = "Test Equipment",
            Model = "Model A",
            SerialNumber = "SN001",
            Manufacturer = "Manufacturer X",
            Category = 1,
            Quantity = 1,
            Description = "Test description",
            CreatedAt = DateTime.UtcNow,
            SecurityLevels = new List<EquipmentSecurityLevel>(),
            InspectionMethods = new List<EquipmentInspectionMethod>()
        };

        // Сохраняем оборудование в базу данных
        await _context.Equipment.AddAsync(equipment);
        await _context.SaveChangesAsync();

        // Act
        await _service.DeleteAsync(equipment.Id);

        // Assert
        var deletedEquipment = await _context.Equipment.FindAsync(equipment.Id);
        Assert.Null(deletedEquipment);
    }

    [Fact]
    public async Task DeleteEquipment_WithMultipleComponents_DeletesAllComponents()
    {
        // Arrange
        var mainEquipment = new Equipment
        {
            Id = Guid.NewGuid(),
            Name = "Main Equipment",
            Model = "Model A",
            SerialNumber = "SN001",
            Manufacturer = "Manufacturer X",
            Category = 1,
            Quantity = 1,
            Description = "Main equipment description",
            CreatedAt = DateTime.UtcNow,
            SecurityLevels = new List<EquipmentSecurityLevel>(),
            InspectionMethods = new List<EquipmentInspectionMethod>(),
            Components = new List<Equipment>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Component 1",
                    Model = "Model B",
                    SerialNumber = "SN002",
                    Manufacturer = "Manufacturer Y",
                    Category = 1,
                    Quantity = 2,
                    Description = "Component 1 description",
                    CreatedAt = DateTime.UtcNow,
                    SecurityLevels = new List<EquipmentSecurityLevel>(),
                    InspectionMethods = new List<EquipmentInspectionMethod>()
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Component 2",
                    Model = "Model C",
                    SerialNumber = "SN003",
                    Manufacturer = "Manufacturer Z",
                    Category = 1,
                    Quantity = 1,
                    Description = "Component 2 description",
                    CreatedAt = DateTime.UtcNow,
                    SecurityLevels = new List<EquipmentSecurityLevel>(),
                    InspectionMethods = new List<EquipmentInspectionMethod>(),
                    Components = new List<Equipment>
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Name = "Sub-component 2.1",
                            Model = "Model D",
                            SerialNumber = "SN004",
                            Manufacturer = "Manufacturer W",
                            Category = 1,
                            Quantity = 1,
                            Description = "Sub-component 2.1 description",
                            CreatedAt = DateTime.UtcNow,
                            SecurityLevels = new List<EquipmentSecurityLevel>(),
                            InspectionMethods = new List<EquipmentInspectionMethod>()
                        }
                    }
                }
            }
        };

        // Сохраняем оборудование в базу данных
        await _context.Equipment.AddAsync(mainEquipment);
        await _context.SaveChangesAsync();

        // Act
        await _service.DeleteAsync(mainEquipment.Id);

        // Assert
        // Проверяем, что основное оборудование удалено
        var deletedMainEquipment = await _context.Equipment.FindAsync(mainEquipment.Id);
        Assert.Null(deletedMainEquipment);

        // Проверяем, что все компоненты удалены
        var allEquipment = await _context.Equipment.ToListAsync();
        Assert.Empty(allEquipment);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
} 