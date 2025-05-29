using AuthApp.application.Services;
using AuthApp.domain.Entities;
using AuthApp.infrastructure.Data;
using AuthApp.infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit;

namespace ServiceTrack.Tests.Integration;

public class EquipmentCascadeDeleteTests : IDisposable
{
    private readonly ApplicationDbContext _context;
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
        _service = new EquipmentService(_repository, _serviceLogger);
    }

    [Fact]
    public async Task DeleteEquipment_WithComponents_DeletesAllComponents()
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
                    InspectionMethods = new List<EquipmentInspectionMethod>(),
                    Components = new List<Equipment>
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Name = "Sub-component 1.1",
                            Model = "Model C",
                            SerialNumber = "SN003",
                            Manufacturer = "Manufacturer Z",
                            Category = 1,
                            Quantity = 1,
                            Description = "Sub-component description",
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