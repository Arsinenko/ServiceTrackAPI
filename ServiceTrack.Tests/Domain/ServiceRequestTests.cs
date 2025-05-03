using AuthApp.domain.Entities;
using Xunit;

namespace ServiceTrack.Tests.Domain;

public class ServiceRequestTests
{
    [Fact]
    public void ServiceRequest_WhenCreated_ShouldHaveCorrectInitialState()
    {
        // Arrange
        var customer = "Test Customer";
        var description = "Test Description";
        var contractId = 1;
        var jobTypeId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        // Act
        var serviceRequest = new ServiceRequest
        {
            Customer = customer,
            Description = description,
            ContractId = contractId,
            JobTypeId = jobTypeId,
            CreatedAt = createdAt
        };

        // Assert
        Assert.Equal(customer, serviceRequest.Customer);
        Assert.Equal(description, serviceRequest.Description);
        Assert.Equal(contractId, serviceRequest.ContractId);
        Assert.Equal(jobTypeId, serviceRequest.JobTypeId);
        Assert.False(serviceRequest.IsCompleted);
        Assert.Null(serviceRequest.CompletedAt);
        Assert.Equal(createdAt, serviceRequest.CreatedAt);
        Assert.Null(serviceRequest.UpdatedAt);
    }

    [Fact]
    public void ServiceRequest_WhenCompleted_ShouldUpdateCompletionState()
    {
        // Arrange
        var serviceRequest = new ServiceRequest
        {
            Customer = "Test Customer",
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var completedAt = DateTime.UtcNow;
        serviceRequest.IsCompleted = true;
        serviceRequest.CompletedAt = completedAt;

        // Assert
        Assert.True(serviceRequest.IsCompleted);
        Assert.Equal(completedAt, serviceRequest.CompletedAt);
    }

    [Fact]
    public void ServiceRequest_WhenUpdated_ShouldUpdateUpdatedAt()
    {
        // Arrange
        var serviceRequest = new ServiceRequest
        {
            Customer = "Test Customer",
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var updatedAt = DateTime.UtcNow;
        serviceRequest.Description = "Updated Description";
        serviceRequest.UpdatedAt = updatedAt;

        // Assert
        Assert.Equal("Updated Description", serviceRequest.Description);
        Assert.Equal(updatedAt, serviceRequest.UpdatedAt);
    }
} 