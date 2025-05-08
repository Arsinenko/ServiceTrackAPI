using AuthApp.application.DTOs;
using AuthApp.application.Interfaces;
using AuthApp.application.Services;
using AuthApp.domain.Entities;
using Moq;
using Xunit;

namespace ServiceTrack.Tests.Application;

public class ServiceRequestServiceTests
{
    private readonly Mock<IServiceRequestRepository> _serviceRequestRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IEquipmentRepository> _equipmentRepositoryMock;
    private readonly Mock<IJobTypeRepository> _jobTypeRepositoryMock;
    private readonly ServiceRequestService _service;

    public ServiceRequestServiceTests()
    {
        _serviceRequestRepositoryMock = new Mock<IServiceRequestRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _equipmentRepositoryMock = new Mock<IEquipmentRepository>();
        _jobTypeRepositoryMock = new Mock<IJobTypeRepository>();
        
        _service = new ServiceRequestService(
            _serviceRequestRepositoryMock.Object,
            _userRepositoryMock.Object,
            _equipmentRepositoryMock.Object,
            _jobTypeRepositoryMock.Object
        );
    }

    [Fact]
    public async Task GetByIdAsync_WhenRequestExists_ReturnsServiceRequestDto()
    {
        // Arrange
        var requestId = 1;
        var serviceRequest = new ServiceRequest
        {
            Id = requestId,
            Customer = "Test Customer",
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow,
            JobType = new JobType { Id = Guid.NewGuid(), Name = "Test Job Type", Description = "Test Description" }
        };

        _serviceRequestRepositoryMock
            .Setup(repo => repo.GetByIdAsync(requestId))
            .ReturnsAsync(serviceRequest);

        // Act
        var result = await _service.GetByIdAsync(requestId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(requestId, result.Id);
        Assert.Equal(serviceRequest.Customer, result.Customer);
        Assert.Equal(serviceRequest.Description, result.Description);
    }

    [Fact]
    public async Task GetByIdAsync_WhenRequestDoesNotExist_ReturnsNull()
    {
        // Arrange
        var requestId = 1;
        _serviceRequestRepositoryMock
            .Setup(repo => repo.GetByIdAsync(requestId))
            .ReturnsAsync((ServiceRequest)null);

        // Act
        var result = await _service.GetByIdAsync(requestId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_CreatesNewServiceRequest()
    {
        // Arrange
        var jobTypeId = Guid.NewGuid();
        var jobType = new JobType { Id = jobTypeId, Name = "Test Job Type", Description = "Test Description" };
        var createDto = new CreateServiceRequestDto
        {
            ContractId = 1,
            Customer = "Test Customer",
            Description = "Test Description",
            JobTypeId = jobTypeId,
            InitialAssignments = new List<InitialUserAssignmentDto>(),
            InitialEquipment = new List<InitialEquipmentAssignmentDto>()
        };

        _jobTypeRepositoryMock
            .Setup(repo => repo.GetByIdAsync(jobTypeId))
            .ReturnsAsync(jobType);

        var createdRequest = new ServiceRequest
        {
            Id = 1,
            ContractId = createDto.ContractId,
            Customer = createDto.Customer,
            Description = createDto.Description,
            JobTypeId = jobTypeId,
            JobType = jobType,
            CreatedAt = DateTime.UtcNow,
            IsCompleted = false,
            UserServiceRequests = new List<UserServiceRequest>(),
            ServiceRequestEquipments = new List<ServiceRequestEquipment>()
        };

        _serviceRequestRepositoryMock
            .Setup(repo => repo.CreateAsync(It.IsAny<ServiceRequest>()))
            .ReturnsAsync(1);

        _serviceRequestRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(createdRequest);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createDto.Customer, result.Customer);
        Assert.Equal(createDto.Description, result.Description);
        Assert.Equal(jobTypeId, result.JobType.Id);
        Assert.False(result.IsCompleted);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidJobType_ThrowsArgumentException()
    {
        // Arrange
        var jobTypeId = Guid.NewGuid();
        var createDto = new CreateServiceRequestDto
        {
            ContractId = 1,
            Customer = "Test Customer",
            Description = "Test Description",
            JobTypeId = jobTypeId,
            InitialAssignments = new List<InitialUserAssignmentDto>(),
            InitialEquipment = new List<InitialEquipmentAssignmentDto>()
        };

        _jobTypeRepositoryMock
            .Setup(repo => repo.GetByIdAsync(jobTypeId))
            .ReturnsAsync((JobType)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_UpdatesServiceRequest()
    {
        // Arrange
        var requestId = 1;
        var jobTypeId = Guid.NewGuid();
        var existingRequest = new ServiceRequest
        {
            Id = requestId,
            Customer = "Old Customer",
            Description = "Old Description",
            JobTypeId = jobTypeId,
            CreatedAt = DateTime.UtcNow,
            UserServiceRequests = new List<UserServiceRequest>(),
            ServiceRequestEquipments = new List<ServiceRequestEquipment>(),
            JobType = new JobType { Id = jobTypeId, Name = "Test Job Type", Description = "Test Description" }
        };

        var jobType = new JobType { Id = jobTypeId, Name = "Test Job Type", Description = "Test Description" };
        var updateDto = new UpdateServiceRequestDto
        {
            Customer = "New Customer",
            Description = "New Description",
            JobTypeId = jobTypeId,
            IsCompleted = true
        };

        _serviceRequestRepositoryMock
            .Setup(repo => repo.GetByIdAsync(requestId))
            .ReturnsAsync(existingRequest);

        _jobTypeRepositoryMock
            .Setup(repo => repo.GetByIdAsync(jobTypeId))
            .ReturnsAsync(jobType);

        _serviceRequestRepositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<ServiceRequest>()))
            .ReturnsAsync(requestId);

        // Act
        var result = await _service.UpdateAsync(requestId, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updateDto.Customer, result.Customer);
        Assert.Equal(updateDto.Description, result.Description);
        Assert.Equal(jobTypeId, result.JobType.Id);
        Assert.Equal(updateDto.IsCompleted, result.IsCompleted);
    }

    [Fact]
    public async Task DeleteAsync_WhenRequestExists_ReturnsTrue()
    {
        // Arrange
        var requestId = 1;
        _serviceRequestRepositoryMock
            .Setup(repo => repo.DeleteAsync(requestId))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteAsync(requestId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CreateBulkAsync_WithValidData_CreatesMultipleServiceRequests()
    {
        // Arrange
        var jobTypeId = Guid.NewGuid();
        var jobType = new JobType { Id = jobTypeId, Name = "Test Job Type", Description = "Test Description" };
        
        var createBulkDto = new CreateServiceRequestBulkDto
        {
            ServiceRequests = new List<CreateServiceRequestDto>
            {
                new()
                {
                    ContractId = 1,
                    Customer = "Test Customer 1",
                    Description = "Test Description 1",
                    JobTypeId = jobTypeId,
                    InitialAssignments = new List<InitialUserAssignmentDto>(),
                    InitialEquipment = new List<InitialEquipmentAssignmentDto>()
                },
                new()
                {
                    ContractId = 2,
                    Customer = "Test Customer 2",
                    Description = "Test Description 2",
                    JobTypeId = jobTypeId,
                    InitialAssignments = new List<InitialUserAssignmentDto>(),
                    InitialEquipment = new List<InitialEquipmentAssignmentDto>()
                }
            }
        };

        _jobTypeRepositoryMock
            .Setup(repo => repo.GetByIdAsync(jobTypeId))
            .ReturnsAsync(jobType);

        _serviceRequestRepositoryMock
            .Setup(repo => repo.CreateBulkAsync(It.IsAny<IEnumerable<ServiceRequest>>()))
            .ReturnsAsync(new List<int> { 1, 2 });

        // Act
        var result = await _service.CreateBulkAsync(createBulkDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.Customer == "Test Customer 1" && r.Description == "Test Description 1");
        Assert.Contains(result, r => r.Customer == "Test Customer 2" && r.Description == "Test Description 2");
    }

    [Fact]
    public async Task CreateBulkAsync_WithInvalidJobType_ThrowsArgumentException()
    {
        // Arrange
        var jobTypeId = Guid.NewGuid();
        var createBulkDto = new CreateServiceRequestBulkDto
        {
            ServiceRequests = new List<CreateServiceRequestDto>
            {
                new()
                {
                    ContractId = 1,
                    Customer = "Test Customer",
                    Description = "Test Description",
                    JobTypeId = jobTypeId,
                    InitialAssignments = new List<InitialUserAssignmentDto>(),
                    InitialEquipment = new List<InitialEquipmentAssignmentDto>()
                }
            }
        };

        _jobTypeRepositoryMock
            .Setup(repo => repo.GetByIdAsync(jobTypeId))
            .ReturnsAsync((JobType)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateBulkAsync(createBulkDto));
    }

    [Fact]
    public async Task CreateBulkAsync_WithEmptyList_ReturnsEmptyList()
    {
        // Arrange
        var createBulkDto = new CreateServiceRequestBulkDto
        {
            ServiceRequests = new List<CreateServiceRequestDto>()
        };

        _serviceRequestRepositoryMock
            .Setup(repo => repo.CreateBulkAsync(It.IsAny<IEnumerable<ServiceRequest>>()))
            .ReturnsAsync(new List<int>());

        // Act
        var result = await _service.CreateBulkAsync(createBulkDto);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
} 