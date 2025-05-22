using AuthApp.application.DTOs;
using AuthApp.application.Exceptions;
using AuthApp.application.Interfaces;
using AuthApp.application.Services;
using AuthApp.domain.Entities;
using Moq;
using Xunit;

namespace ServiceTrack.Tests.Application;

public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly CustomerService _service;

    public CustomerServiceTests()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _service = new CustomerService(_customerRepositoryMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCustomerExists_ReturnsCustomerDto()
    {
        // Arrange
        var customerId = 1;
        var createdAt = DateTime.UtcNow;
        var updatedAt = DateTime.UtcNow;
        var customer = new Customer
        {
            Id = customerId,
            Name = "Test Customer",
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        _customerRepositoryMock
            .Setup(repo => repo.GetByIdAsync(customerId))
            .ReturnsAsync(customer);

        // Act
        var result = await _service.GetByIdAsync(customerId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customerId, result.Id);
        Assert.Equal(customer.Name, result.Name);
        Assert.Equal(createdAt.ToUniversalTime(), result.CreatedAt.ToUniversalTime());
        Assert.Equal(updatedAt.ToUniversalTime(), result.UpdatedAt.ToUniversalTime());
    }

    [Fact]
    public async Task GetByIdAsync_WhenCustomerDoesNotExist_ReturnsNull()
    {
        // Arrange
        var customerId = 1;
        _customerRepositoryMock
            .Setup(repo => repo.GetByIdAsync(customerId))
            .ReturnsAsync((Customer)null);

        // Act
        var result = await _service.GetByIdAsync(customerId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCustomers()
    {
        // Arrange
        var customers = new List<Customer>
        {
            new()
            {
                Id = 1,
                Name = "Customer 1",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = 2,
                Name = "Customer 2",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _customerRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(customers);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, c => c.Id == 1 && c.Name == "Customer 1");
        Assert.Contains(result, c => c.Id == 2 && c.Name == "Customer 2");
    }

    [Fact]
    public async Task CreateAsync_WithValidData_CreatesNewCustomer()
    {
        // Arrange
        var createDto = new CreateCustomerDto
        {
            Name = "New Customer"
        };

        var createdAt = DateTime.UtcNow;
        var createdCustomer = new Customer
        {
            Id = 1,
            Name = createDto.Name,
            CreatedAt = createdAt
        };

        _customerRepositoryMock
            .Setup(repo => repo.GetByNameAsync(createDto.Name))
            .ReturnsAsync((Customer)null);

        _customerRepositoryMock
            .Setup(repo => repo.CreateAsync(It.IsAny<Customer>()))
            .ReturnsAsync(createdCustomer);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createDto.Name, result.Name);
        Assert.Equal(createdAt, result.CreatedAt.ToUniversalTime());
    }

    [Fact]
    public async Task CreateAsync_WithExistingName_ThrowsCustomerAlreadyExistsException()
    {
        // Arrange
        var createDto = new CreateCustomerDto
        {
            Name = "Existing Customer"
        };

        var existingCustomer = new Customer
        {
            Id = 1,
            Name = createDto.Name,
            CreatedAt = DateTime.UtcNow
        };

        _customerRepositoryMock
            .Setup(repo => repo.GetByNameAsync(createDto.Name))
            .ReturnsAsync(existingCustomer);

        // Act & Assert
        await Assert.ThrowsAsync<CustomerAlreadyExistsException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateBulkAsync_WithValidData_CreatesMultipleCustomers()
    {
        // Arrange
        var createBulkDto = new CreateCustomerBulkDto
        {
            Customers = new List<CreateCustomerDto>
            {
                new() { Name = "Customer 1" },
                new() { Name = "Customer 2" }
            }
        };

        var createdCustomers = new List<Customer>
        {
            new() { Id = 1, Name = "Customer 1", CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Name = "Customer 2", CreatedAt = DateTime.UtcNow }
        };

        _customerRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(new List<Customer>());

        _customerRepositoryMock
            .Setup(repo => repo.CreateBulkAsync(It.IsAny<IEnumerable<Customer>>()))
            .ReturnsAsync(createdCustomers);

        // Act
        var result = await _service.CreateBulkAsync(createBulkDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.CreatedCustomers.Count);
        Assert.Empty(result.FiledCustomers);
        Assert.Empty(result.FailureReasons);
        Assert.Contains(result.CreatedCustomers, c => c.Name == "Customer 1");
        Assert.Contains(result.CreatedCustomers, c => c.Name == "Customer 2");
    }

    [Fact]
    public async Task CreateBulkAsync_WithDuplicateNames_ReportsFailures()
    {
        // Arrange
        var createBulkDto = new CreateCustomerBulkDto
        {
            Customers = new List<CreateCustomerDto>
            {
                new() { Name = "Customer 1" },
                new() { Name = "Customer 1" },
                new() { Name = "Customer 2" }
            }
        };

        var existingCustomers = new List<Customer>
        {
            new() { Id = 1, Name = "Customer 2", CreatedAt = DateTime.UtcNow }
        };

        _customerRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(existingCustomers);

        // Act
        var result = await _service.CreateBulkAsync(createBulkDto);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.CreatedCustomers);
        Assert.Equal(3, result.FiledCustomers.Count);
        Assert.Equal(3, result.FailureReasons.Count);
        Assert.Contains(result.FailureReasons, r => r.Contains("duplicate in the batch"));
        Assert.Contains(result.FailureReasons, r => r.Contains("already exists"));
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_UpdatesCustomer()
    {
        // Arrange
        var customerId = 1;
        var updateDto = new UpdateCustomerDto
        {
            Id = customerId,
            Name = "Updated Customer"
        };

        var createdAt = DateTime.UtcNow;
        var updatedAt = DateTime.UtcNow;
        var existingCustomer = new Customer
        {
            Id = customerId,
            Name = "Original Customer",
            CreatedAt = createdAt
        };

        var updatedCustomer = new Customer
        {
            Id = customerId,
            Name = updateDto.Name,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        _customerRepositoryMock
            .Setup(repo => repo.GetByIdAsync(customerId))
            .ReturnsAsync(existingCustomer);

        _customerRepositoryMock
            .Setup(repo => repo.GetByNameAsync(updateDto.Name))
            .ReturnsAsync((Customer)null);

        _customerRepositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Customer>()))
            .ReturnsAsync(updatedCustomer);

        // Act
        var result = await _service.UpdateAsync(updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updateDto.Name, result.Name);
        Assert.Equal(updatedAt.ToUniversalTime(), result.UpdatedAt.ToUniversalTime());
    }

    [Fact]
    public async Task UpdateAsync_WhenCustomerDoesNotExist_ThrowsCustomerNotFoundException()
    {
        // Arrange
        var updateDto = new UpdateCustomerDto
        {
            Id = 1,
            Name = "Updated Customer"
        };

        _customerRepositoryMock
            .Setup(repo => repo.GetByIdAsync(updateDto.Id))
            .ReturnsAsync((Customer)null);

        // Act & Assert
        await Assert.ThrowsAsync<CustomerNotFoundException>(() => _service.UpdateAsync(updateDto));
    }

    [Fact]
    public async Task UpdateAsync_WithExistingName_ThrowsCustomerAlreadyExistsException()
    {
        // Arrange
        var customerId = 1;
        var updateDto = new UpdateCustomerDto
        {
            Id = customerId,
            Name = "Existing Customer"
        };

        var existingCustomer = new Customer
        {
            Id = customerId,
            Name = "Original Customer",
            CreatedAt = DateTime.UtcNow
        };

        var customerWithSameName = new Customer
        {
            Id = 2,
            Name = updateDto.Name,
            CreatedAt = DateTime.UtcNow
        };

        _customerRepositoryMock
            .Setup(repo => repo.GetByIdAsync(customerId))
            .ReturnsAsync(existingCustomer);

        _customerRepositoryMock
            .Setup(repo => repo.GetByNameAsync(updateDto.Name))
            .ReturnsAsync(customerWithSameName);

        // Act & Assert
        await Assert.ThrowsAsync<CustomerAlreadyExistsException>(() => _service.UpdateAsync(updateDto));
    }

    [Fact]
    public async Task UpdateBulkAsync_WithValidData_UpdatesMultipleCustomers()
    {
        // Arrange
        var updateBulkDto = new UpdateCustomerBulkDto
        {
            Customers = new List<UpdateCustomerDto>
            {
                new() { Id = 1, Name = "Updated Customer 1" },
                new() { Id = 2, Name = "Updated Customer 2" }
            }
        };

        var createdAt = DateTime.UtcNow;
        var updatedAt = DateTime.UtcNow;
        var existingCustomers = new List<Customer>
        {
            new() { Id = 1, Name = "Original Customer 1", CreatedAt = createdAt },
            new() { Id = 2, Name = "Original Customer 2", CreatedAt = createdAt }
        };

        var updatedCustomers = new List<Customer>
        {
            new() { Id = 1, Name = "Updated Customer 1", CreatedAt = createdAt, UpdatedAt = updatedAt },
            new() { Id = 2, Name = "Updated Customer 2", CreatedAt = createdAt, UpdatedAt = updatedAt }
        };

        _customerRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(existingCustomers[0]);
        _customerRepositoryMock
            .Setup(repo => repo.GetByIdAsync(2))
            .ReturnsAsync(existingCustomers[1]);
        _customerRepositoryMock
            .Setup(repo => repo.GetByNameAsync("Updated Customer 1"))
            .ReturnsAsync((Customer)null);
        _customerRepositoryMock
            .Setup(repo => repo.GetByNameAsync("Updated Customer 2"))
            .ReturnsAsync((Customer)null);
        _customerRepositoryMock
            .Setup(repo => repo.UpdateBulkAsync(It.Is<IEnumerable<Customer>>(c => 
                c.Count() == 2 && 
                c.Any(x => x.Id == 1 && x.Name == "Updated Customer 1") &&
                c.Any(x => x.Id == 2 && x.Name == "Updated Customer 2"))))
            .ReturnsAsync(updatedCustomers);

        // Act
        var result = await _service.UpdateBulkAsync(updateBulkDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, c => c.Id == 1 && c.Name == "Updated Customer 1" && c.UpdatedAt.ToUniversalTime() == updatedAt.ToUniversalTime());
        Assert.Contains(result, c => c.Id == 2 && c.Name == "Updated Customer 2" && c.UpdatedAt.ToUniversalTime() == updatedAt.ToUniversalTime());
    }

    [Fact]
    public async Task UpdateBulkAsync_WithNonExistentCustomer_SkipsUpdate()
    {
        // Arrange
        var updateBulkDto = new UpdateCustomerBulkDto
        {
            Customers = new List<UpdateCustomerDto>
            {
                new() { Id = 1, Name = "Updated Customer 1" },
                new() { Id = 999, Name = "Non-existent Customer" }
            }
        };

        var createdAt = DateTime.UtcNow;
        var updatedAt = DateTime.UtcNow;
        var existingCustomer = new Customer
        {
            Id = 1,
            Name = "Original Customer 1",
            CreatedAt = createdAt
        };

        var updatedCustomer = new Customer
        {
            Id = 1,
            Name = "Updated Customer 1",
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        _customerRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(existingCustomer);
        _customerRepositoryMock
            .Setup(repo => repo.GetByIdAsync(999))
            .ReturnsAsync((Customer)null);
        _customerRepositoryMock
            .Setup(repo => repo.GetByNameAsync("Updated Customer 1"))
            .ReturnsAsync((Customer)null);
        _customerRepositoryMock
            .Setup(repo => repo.UpdateBulkAsync(It.Is<IEnumerable<Customer>>(c => 
                c.Count() == 1 && 
                c.Any(x => x.Id == 1 && x.Name == "Updated Customer 1"))))
            .ReturnsAsync(new List<Customer> { updatedCustomer });

        // Act
        var result = await _service.UpdateBulkAsync(updateBulkDto);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Contains(result, c => c.Id == 1 && c.Name == "Updated Customer 1" && c.UpdatedAt.ToUniversalTime() == updatedAt.ToUniversalTime());
    }

    [Fact]
    public async Task UpdateBulkAsync_WithDuplicateName_SkipsUpdate()
    {
        // Arrange
        var updateBulkDto = new UpdateCustomerBulkDto
        {
            Customers = new List<UpdateCustomerDto>
            {
                new() { Id = 1, Name = "Updated Customer 1" },
                new() { Id = 2, Name = "Existing Name" }
            }
        };

        var createdAt = DateTime.UtcNow;
        var existingCustomers = new List<Customer>
        {
            new() { Id = 1, Name = "Original Customer 1", CreatedAt = createdAt },
            new() { Id = 2, Name = "Original Customer 2", CreatedAt = createdAt }
        };

        var customerWithSameName = new Customer
        {
            Id = 3,
            Name = "Existing Name",
            CreatedAt = createdAt
        };

        var updatedCustomer = new Customer
        {
            Id = 1,
            Name = "Updated Customer 1",
            CreatedAt = createdAt,
            UpdatedAt = DateTime.UtcNow
        };

        _customerRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(existingCustomers[0]);
        _customerRepositoryMock
            .Setup(repo => repo.GetByIdAsync(2))
            .ReturnsAsync(existingCustomers[1]);
        _customerRepositoryMock
            .Setup(repo => repo.GetByNameAsync("Updated Customer 1"))
            .ReturnsAsync((Customer)null);
        _customerRepositoryMock
            .Setup(repo => repo.GetByNameAsync("Existing Name"))
            .ReturnsAsync(customerWithSameName);
        _customerRepositoryMock
            .Setup(repo => repo.UpdateBulkAsync(It.Is<IEnumerable<Customer>>(c => 
                c.Count() == 1 && 
                c.Any(x => x.Id == 1 && x.Name == "Updated Customer 1"))))
            .ReturnsAsync(new List<Customer> { updatedCustomer });

        // Act
        var result = await _service.UpdateBulkAsync(updateBulkDto);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Contains(result, c => c.Id == 1 && c.Name == "Updated Customer 1");
    }

    [Fact]
    public async Task DeleteAsync_WhenCustomerExists_DeletesCustomer()
    {
        // Arrange
        var customerId = 1;
        var customer = new Customer
        {
            Id = customerId,
            Name = "Test Customer",
            CreatedAt = DateTime.UtcNow
        };

        _customerRepositoryMock
            .Setup(repo => repo.GetByIdAsync(customerId))
            .ReturnsAsync(customer);

        _customerRepositoryMock
            .Setup(repo => repo.DeleteAsync(customerId))
            .ReturnsAsync(customer);

        // Act
        var result = await _service.DeleteAsync(new DeleteCustomerDto { Id = customerId });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customerId, result.Id);
        Assert.Equal(customer.Name, result.Name);
        _customerRepositoryMock.Verify(repo => repo.DeleteAsync(customerId), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenCustomerDoesNotExist_ThrowsCustomerNotFoundException()
    {
        // Arrange
        var customerId = 1;
        _customerRepositoryMock
            .Setup(repo => repo.GetByIdAsync(customerId))
            .ReturnsAsync((Customer)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CustomerNotFoundException>(() => 
            _service.DeleteAsync(new DeleteCustomerDto { Id = customerId }));
        Assert.Equal($"Customer with id {customerId} does not exist.", exception.Message);
    }

    [Fact]
    public async Task DeleteBulkAsync_WithValidData_DeletesMultipleCustomers()
    {
        // Arrange
        var customerIds = new List<int> { 1, 2 };
        var customers = new List<Customer>
        {
            new() { Id = 1, Name = "Customer 1", CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Name = "Customer 2", CreatedAt = DateTime.UtcNow }
        };

        _customerRepositoryMock
            .Setup(repo => repo.DeleteBulkAsync(customerIds))
            .ReturnsAsync(customers);

        // Act
        var result = await _service.DeleteBulkAsync(new DeleteCustomerBulkDto 
        { 
            Customers = customerIds.Select(id => new DeleteCustomerDto { Id = id }).ToList() 
        });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, c => c.Id == 1 && c.Name == "Customer 1");
        Assert.Contains(result, c => c.Id == 2 && c.Name == "Customer 2");
    }
} 