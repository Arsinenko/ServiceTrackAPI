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
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly ServiceRequestService _service;

    public ServiceRequestServiceTests()
    {
        _serviceRequestRepositoryMock = new Mock<IServiceRequestRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _equipmentRepositoryMock = new Mock<IEquipmentRepository>();
        _jobTypeRepositoryMock = new Mock<IJobTypeRepository>();
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        
        _service = new ServiceRequestService(
            _serviceRequestRepositoryMock.Object,
            _userRepositoryMock.Object,
            _equipmentRepositoryMock.Object,
            _jobTypeRepositoryMock.Object,
            _customerRepositoryMock.Object
        );
    }

    [Fact]
    public async Task GetByIdAsync_WhenRequestExists_ReturnsServiceRequestDto()
    {
        // Arrange
        var requestId = 1;
        var customerId = 1;
        var customer = new Customer { Id = customerId, Name = "Test Customer" };
        var serviceRequest = new ServiceRequest
        {
            Id = requestId,
            CustomerId = customerId,
            Customer = customer,
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow,
            JobType = new JobType { Id = Guid.NewGuid(), Name = "Test Job Type", Description = "Test Job Type Description" }
        };

        _serviceRequestRepositoryMock
            .Setup(repo => repo.GetByIdAsync(requestId))
            .ReturnsAsync(serviceRequest);

        // Act
        var result = await _service.GetByIdAsync(requestId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(requestId, result.Id);
        Assert.Equal(customerId, result.Customer.Id);
        Assert.Equal(customer.Name, result.Customer.Name);
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
        var customerId = 1;
        var jobType = new JobType { Id = jobTypeId, Name = "Test Job Type", Description = "Test Job Type Description" };
        var customer = new Customer { Id = customerId, Name = "Test Customer" };
        var createDto = new CreateServiceRequestDto
        {
            ContractId = 1,
            CustomerId = customerId,
            Description = "Test Description",
            JobTypeId = jobTypeId,
            InitialAssignments = new List<InitialUserAssignmentDto>(),
            InitialEquipment = new List<InitialEquipmentAssignmentDto>()
        };

        _jobTypeRepositoryMock
            .Setup(repo => repo.GetByIdAsync(jobTypeId))
            .ReturnsAsync(jobType);

        _customerRepositoryMock
            .Setup(repo => repo.GetByIdAsync(customerId))
            .ReturnsAsync(customer);

        var createdRequest = new ServiceRequest
        {
            Id = 1,
            ContractId = createDto.ContractId,
            CustomerId = customerId,
            Customer = customer,
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
        Assert.Equal(customerId, result.Customer.Id);
        Assert.Equal(customer.Name, result.Customer.Name);
        Assert.Equal(createDto.Description, result.Description);
        Assert.Equal(jobTypeId, result.JobType.Id);
        Assert.False(result.IsCompleted);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidJobType_ThrowsArgumentException()
    {
        // Arrange
        var jobTypeId = Guid.NewGuid();
        var customerId = 1;
        var createDto = new CreateServiceRequestDto
        {
            ContractId = 1,
            CustomerId = customerId,
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
    public async Task CreateAsync_WithInvalidCustomer_ThrowsArgumentException()
    {
        // Arrange
        var jobTypeId = Guid.NewGuid();
        var customerId = 1;
        var jobType = new JobType { Id = jobTypeId, Name = "Test Job Type", Description = "Test Job Type Description" };
        var createDto = new CreateServiceRequestDto
        {
            ContractId = 1,
            CustomerId = customerId,
            Description = "Test Description",
            JobTypeId = jobTypeId,
            InitialAssignments = new List<InitialUserAssignmentDto>(),
            InitialEquipment = new List<InitialEquipmentAssignmentDto>()
        };

        _jobTypeRepositoryMock
            .Setup(repo => repo.GetByIdAsync(jobTypeId))
            .ReturnsAsync(jobType);

        _customerRepositoryMock
            .Setup(repo => repo.GetByIdAsync(customerId))
            .ReturnsAsync((Customer)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_UpdatesServiceRequest()
    {
        // Arrange
        var requestId = 1;
        var customerId = 1;
        var jobTypeId = Guid.NewGuid();
        var customer = new Customer { Id = customerId, Name = "Test Customer" };
        var jobType = new JobType { Id = jobTypeId, Name = "Test Job Type", Description = "Test Job Type Description" };
        var existingRequest = new ServiceRequest
        {
            Id = requestId,
            CustomerId = customerId,
            Customer = customer,
            Description = "Old Description",
            JobTypeId = jobTypeId,
            JobType = jobType,
            CreatedAt = DateTime.UtcNow,
            UserServiceRequests = new List<UserServiceRequest>(),
            ServiceRequestEquipments = new List<ServiceRequestEquipment>()
        };

        var updateDto = new UpdateServiceRequestDto
        {
            CustomerId = customerId,
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

        _customerRepositoryMock
            .Setup(repo => repo.GetByIdAsync(customerId))
            .ReturnsAsync(customer);

        _serviceRequestRepositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<ServiceRequest>()))
            .ReturnsAsync(requestId);

        // Act
        var result = await _service.UpdateAsync(requestId, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customerId, result.Customer.Id);
        Assert.Equal(customer.Name, result.Customer.Name);
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
        var customerId = 1;
        var jobType = new JobType { Id = jobTypeId, Name = "Test Job Type", Description = "Test Job Type Description" };
        var customer = new Customer { Id = customerId, Name = "Test Customer" };
        
        var createBulkDto = new CreateServiceRequestBulkDto
        {
            ServiceRequests = new List<CreateServiceRequestDto>
            {
                new()
                {
                    ContractId = 1,
                    CustomerId = customerId,
                    Description = "Test Description 1",
                    JobTypeId = jobTypeId,
                    InitialAssignments = new List<InitialUserAssignmentDto>(),
                    InitialEquipment = new List<InitialEquipmentAssignmentDto>()
                },
                new()
                {
                    ContractId = 2,
                    CustomerId = customerId,
                    Description = "Test Description 2",
                    JobTypeId = jobTypeId,
                    InitialAssignments = new List<InitialUserAssignmentDto>(),
                    InitialEquipment = new List<InitialEquipmentAssignmentDto>()
                }
            }
        };

        var createdRequests = new List<ServiceRequest>
        {
            new()
            {
                Id = 1,
                ContractId = 1,
                CustomerId = customerId,
                Customer = customer,
                Description = "Test Description 1",
                JobTypeId = jobTypeId,
                JobType = jobType,
                CreatedAt = DateTime.UtcNow,
                IsCompleted = false,
                UserServiceRequests = new List<UserServiceRequest>(),
                ServiceRequestEquipments = new List<ServiceRequestEquipment>()
            },
            new()
            {
                Id = 2,
                ContractId = 2,
                CustomerId = customerId,
                Customer = customer,
                Description = "Test Description 2",
                JobTypeId = jobTypeId,
                JobType = jobType,
                CreatedAt = DateTime.UtcNow,
                IsCompleted = false,
                UserServiceRequests = new List<UserServiceRequest>(),
                ServiceRequestEquipments = new List<ServiceRequestEquipment>()
            }
        };

        _jobTypeRepositoryMock
            .Setup(repo => repo.GetByIdAsync(jobTypeId))
            .ReturnsAsync(jobType);

        _customerRepositoryMock
            .Setup(repo => repo.GetByIdAsync(customerId))
            .ReturnsAsync(customer);

        _serviceRequestRepositoryMock
            .Setup(repo => repo.CreateBulkAsync(It.IsAny<IEnumerable<ServiceRequest>>()))
            .ReturnsAsync(new List<int> { 1, 2 });

        _serviceRequestRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(createdRequests[0]);

        _serviceRequestRepositoryMock
            .Setup(repo => repo.GetByIdAsync(2))
            .ReturnsAsync(createdRequests[1]);

        // Act
        var result = await _service.CreateBulkAsync(createBulkDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.Customer.Id == customerId && r.Description == "Test Description 1");
        Assert.Contains(result, r => r.Customer.Id == customerId && r.Description == "Test Description 2");
    }

    [Fact]
    public async Task CreateBulkAsync_WithInvalidJobType_ThrowsArgumentException()
    {
        // Arrange
        var jobTypeId = Guid.NewGuid();
        var customerId = 1;
        var createBulkDto = new CreateServiceRequestBulkDto
        {
            ServiceRequests = new List<CreateServiceRequestDto>
            {
                new()
                {
                    ContractId = 1,
                    CustomerId = customerId,
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
    public async Task CreateBulkAsync_WithInvalidCustomer_ThrowsArgumentException()
    {
        // Arrange
        var jobTypeId = Guid.NewGuid();
        var customerId = 1;
        var jobType = new JobType { Id = jobTypeId, Name = "Test Job Type", Description = "Test Job Type Description" };
        var createBulkDto = new CreateServiceRequestBulkDto
        {
            ServiceRequests = new List<CreateServiceRequestDto>
            {
                new()
                {
                    ContractId = 1,
                    CustomerId = customerId,
                    Description = "Test Description",
                    JobTypeId = jobTypeId,
                    InitialAssignments = new List<InitialUserAssignmentDto>(),
                    InitialEquipment = new List<InitialEquipmentAssignmentDto>()
                }
            }
        };

        _jobTypeRepositoryMock
            .Setup(repo => repo.GetByIdAsync(jobTypeId))
            .ReturnsAsync(jobType);

        _customerRepositoryMock
            .Setup(repo => repo.GetByIdAsync(customerId))
            .ReturnsAsync((Customer)null);

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

    [Fact]
    public async Task DeleteBulkAsync_WhenAllRequestsExist_DeletesAllRequests()
    {
        // Arrange
        var requestIds = new List<int> { 1, 2 };
        var customerId = 1;
        var customer = new Customer { Id = customerId, Name = "Test Customer" };
        var requests = new List<ServiceRequest>
        {
            new()
            {
                Id = 1,
                CustomerId = customerId,
                Customer = customer,
                Description = "Description 1",
                CreatedAt = DateTime.UtcNow,
                JobType = new JobType { Id = Guid.NewGuid(), Name = "Test Job Type", Description = "Test Job Type Description" }
            },
            new()
            {
                Id = 2,
                CustomerId = customerId,
                Customer = customer,
                Description = "Description 2",
                CreatedAt = DateTime.UtcNow,
                JobType = new JobType { Id = Guid.NewGuid(), Name = "Test Job Type", Description = "Test Job Type Description" }
            }
        };

        _serviceRequestRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(requests[0]);
        _serviceRequestRepositoryMock
            .Setup(repo => repo.GetByIdAsync(2))
            .ReturnsAsync(requests[1]);
        _serviceRequestRepositoryMock
            .Setup(repo => repo.DeleteAsync(It.IsAny<int>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteBulkAsync(requestIds);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.DeletedServiceRequests.Count());
        Assert.Empty(result.FailedServiceRequestIds);
        Assert.Empty(result.FailureReasons);
        Assert.Contains(result.DeletedServiceRequests, r => r.Id == 1 && r.Customer.Id == customerId);
        Assert.Contains(result.DeletedServiceRequests, r => r.Id == 2 && r.Customer.Id == customerId);
        _serviceRequestRepositoryMock.Verify(repo => repo.DeleteAsync(1), Times.Once);
        _serviceRequestRepositoryMock.Verify(repo => repo.DeleteAsync(2), Times.Once);
    }

    [Fact]
    public async Task DeleteBulkAsync_WhenSomeRequestsDoNotExist_ReturnsPartialSuccess()
    {
        // Arrange
        var existingId = 1;
        var nonExistentId = 2;
        var requestIds = new List<int> { existingId, nonExistentId };
        var customerId = 1;
        var customer = new Customer { Id = customerId, Name = "Test Customer" };
        var existingRequest = new ServiceRequest
        {
            Id = existingId,
            CustomerId = customerId,
            Customer = customer,
            Description = "Existing Description",
            CreatedAt = DateTime.UtcNow,
            JobType = new JobType { Id = Guid.NewGuid(), Name = "Test Job Type", Description = "Test Job Type Description" }
        };

        _serviceRequestRepositoryMock
            .Setup(repo => repo.GetByIdAsync(existingId))
            .ReturnsAsync(existingRequest);
        _serviceRequestRepositoryMock
            .Setup(repo => repo.GetByIdAsync(nonExistentId))
            .ReturnsAsync((ServiceRequest)null);
        _serviceRequestRepositoryMock
            .Setup(repo => repo.DeleteAsync(existingId))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteBulkAsync(requestIds);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.DeletedServiceRequests);
        Assert.Single(result.FailedServiceRequestIds);
        Assert.Single(result.FailureReasons);
        Assert.Contains(result.DeletedServiceRequests, r => r.Id == existingId && r.Customer.Id == customerId);
        Assert.Contains(result.FailedServiceRequestIds, id => id == nonExistentId);
        Assert.Contains(result.FailureReasons, reason => reason == "Request not found");
        _serviceRequestRepositoryMock.Verify(repo => repo.DeleteAsync(existingId), Times.Once);
        _serviceRequestRepositoryMock.Verify(repo => repo.DeleteAsync(nonExistentId), Times.Never);
    }

    [Fact]
    public async Task DeleteBulkAsync_WhenDeleteOperationFails_ReportsFailure()
    {
        // Arrange
        var requestId = 1;
        var customerId = 1;
        var customer = new Customer { Id = customerId, Name = "Test Customer" };
        var request = new ServiceRequest
        {
            Id = requestId,
            CustomerId = customerId,
            Customer = customer,
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow,
            JobType = new JobType { Id = Guid.NewGuid(), Name = "Test Job Type", Description = "Test Job Type Description" }
        };

        _serviceRequestRepositoryMock
            .Setup(repo => repo.GetByIdAsync(requestId))
            .ReturnsAsync(request);
        _serviceRequestRepositoryMock
            .Setup(repo => repo.DeleteAsync(requestId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _service.DeleteBulkAsync(new List<int> { requestId });

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.DeletedServiceRequests);
        Assert.Single(result.FailedServiceRequestIds);
        Assert.Single(result.FailureReasons);
        Assert.Contains(requestId, result.FailedServiceRequestIds);
        Assert.Contains(result.FailureReasons, reason => reason == "Database error");
    }

    [Fact]
    public async Task DeleteBulkAsync_WhenEmptyListProvided_ReturnsEmptyResult()
    {
        // Arrange
        var requestIds = new List<int>();

        // Act
        var result = await _service.DeleteBulkAsync(requestIds);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.DeletedServiceRequests);
        Assert.Empty(result.FailedServiceRequestIds);
        Assert.Empty(result.FailureReasons);
        _serviceRequestRepositoryMock.Verify(repo => repo.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _serviceRequestRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<int>()), Times.Never);
    }
} 