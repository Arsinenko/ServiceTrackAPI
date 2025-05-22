using AuthApp.domain.Entities;
using Xunit;

namespace ServiceTrack.Tests.Domain;

public class ServiceRequestTests
{
    [Fact]
    public void ServiceRequest_WithValidData_CreatesSuccessfully()
    {
        // Arrange
        var customerId = 1;
        var customer = new Customer { Id = customerId, Name = "Test Customer" };
        var jobTypeId = Guid.NewGuid();
        var jobType = new JobType { Id = jobTypeId, Name = "Test Job Type", Description = "Test Job Type Description" };

        // Act
        var serviceRequest = new ServiceRequest
        {
            Id = 1,
            CustomerId = customerId,
            Customer = customer,
            Description = "Test Description",
            JobTypeId = jobTypeId,
            JobType = jobType,
            CreatedAt = DateTime.UtcNow,
            IsCompleted = false
        };

        // Assert
        Assert.Equal(1, serviceRequest.Id);
        Assert.Equal(customerId, serviceRequest.CustomerId);
        Assert.Equal(customer, serviceRequest.Customer);
        Assert.Equal("Test Description", serviceRequest.Description);
        Assert.Equal(jobTypeId, serviceRequest.JobTypeId);
        Assert.Equal(jobType, serviceRequest.JobType);
        Assert.False(serviceRequest.IsCompleted);
    }

    [Fact]
    public void ServiceRequest_WithMinimalData_CreatesSuccessfully()
    {
        // Arrange
        var customerId = 1;
        var customer = new Customer { Id = customerId, Name = "Test Customer" };
        var jobTypeId = Guid.NewGuid();
        var jobType = new JobType { Id = jobTypeId, Name = "Test Job Type", Description = "Test Job Type Description" };

        // Act
        var serviceRequest = new ServiceRequest
        {
            CustomerId = customerId,
            Customer = customer,
            JobTypeId = jobTypeId,
            JobType = jobType
        };

        // Assert
        Assert.Equal(customerId, serviceRequest.CustomerId);
        Assert.Equal(customer, serviceRequest.Customer);
        Assert.Equal(jobTypeId, serviceRequest.JobTypeId);
        Assert.Equal(jobType, serviceRequest.JobType);
        Assert.False(serviceRequest.IsCompleted);
        Assert.NotNull(serviceRequest.CreatedAt);
    }

    [Fact]
    public void ServiceRequest_WithCompletedStatus_SetsCorrectly()
    {
        // Arrange
        var customerId = 1;
        var customer = new Customer { Id = customerId, Name = "Test Customer" };
        var jobTypeId = Guid.NewGuid();
        var jobType = new JobType { Id = jobTypeId, Name = "Test Job Type", Description = "Test Job Type Description" };

        // Act
        var serviceRequest = new ServiceRequest
        {
            CustomerId = customerId,
            Customer = customer,
            JobTypeId = jobTypeId,
            JobType = jobType,
            IsCompleted = true
        };

        // Assert
        Assert.Equal(customerId, serviceRequest.CustomerId);
        Assert.Equal(customer, serviceRequest.Customer);
        Assert.Equal(jobTypeId, serviceRequest.JobTypeId);
        Assert.Equal(jobType, serviceRequest.JobType);
        Assert.True(serviceRequest.IsCompleted);
    }
} 