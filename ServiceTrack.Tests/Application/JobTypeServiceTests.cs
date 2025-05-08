using AuthApp.application.DTOs;
using AuthApp.application.Interfaces;
using AuthApp.application.Services;
using AuthApp.domain.Entities;
using Moq;
using Xunit;

namespace ServiceTrack.Tests.Application;

public class JobTypeServiceTests
{
    private readonly Mock<IJobTypeRepository> _jobTypeRepositoryMock;
    private readonly JobTypeService _service;

    public JobTypeServiceTests()
    {
        _jobTypeRepositoryMock = new Mock<IJobTypeRepository>();
        _service = new JobTypeService(_jobTypeRepositoryMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenJobTypeExists_ReturnsJobTypeDto()
    {
        // Arrange
        var jobTypeId = Guid.NewGuid();
        var jobType = new JobType
        {
            Id = jobTypeId,
            Name = "Test Job Type",
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _jobTypeRepositoryMock
            .Setup(repo => repo.GetByIdAsync(jobTypeId))
            .ReturnsAsync(jobType);

        // Act
        var result = await _service.GetByIdAsync(jobTypeId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(jobTypeId, result.Id);
        Assert.Equal(jobType.Name, result.Name);
        Assert.Equal(jobType.Description, result.Description);
    }

    [Fact]
    public async Task GetByIdAsync_WhenJobTypeDoesNotExist_ReturnsNull()
    {
        // Arrange
        var jobTypeId = Guid.NewGuid();
        _jobTypeRepositoryMock
            .Setup(repo => repo.GetByIdAsync(jobTypeId))
            .ReturnsAsync((JobType)null);

        // Act
        var result = await _service.GetByIdAsync(jobTypeId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNameAsync_WhenJobTypeExists_ReturnsJobTypeDto()
    {
        // Arrange
        var jobTypeName = "Test Job Type";
        var jobType = new JobType
        {
            Id = Guid.NewGuid(),
            Name = jobTypeName,
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _jobTypeRepositoryMock
            .Setup(repo => repo.GetByNameAsync(jobTypeName))
            .ReturnsAsync(jobType);

        // Act
        var result = await _service.GetBeyNameAsync(jobTypeName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(jobTypeName, result.Name);
        Assert.Equal(jobType.Description, result.Description);
    }

    [Fact]
    public async Task GetByNameAsync_WhenJobTypeDoesNotExist_ReturnsNull()
    {
        // Arrange
        var jobTypeName = "Non-existent Job Type";
        _jobTypeRepositoryMock
            .Setup(repo => repo.GetByNameAsync(jobTypeName))
            .ReturnsAsync((JobType)null);

        // Act
        var result = await _service.GetBeyNameAsync(jobTypeName);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllJobTypes()
    {
        // Arrange
        var jobTypes = new List<JobType>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Job Type 1",
                Description = "Description 1",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Job Type 2",
                Description = "Description 2",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _jobTypeRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(jobTypes);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, jt => jt.Name == "Job Type 1");
        Assert.Contains(result, jt => jt.Name == "Job Type 2");
    }

    [Fact]
    public async Task CreateAsync_ValidData_CreatesAndReturnsJobTypeDto()
    {
        // Arrange
        var createDto = new CreateJobTypeDto
        {
            Name = "New Job Type",
            Description = "New Description"
        };

        _jobTypeRepositoryMock
            .Setup(repo => repo.CreateAsync(It.IsAny<JobType>()))
            .ReturnsAsync(Guid.NewGuid());

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createDto.Name, result.Name);
        Assert.Equal(createDto.Description, result.Description);
    }

    [Fact]
    public async Task UpdateAsync_WhenJobTypeExists_UpdatesAndReturnsJobTypeDto()
    {
        // Arrange
        var jobTypeId = Guid.NewGuid();
        var existingJobType = new JobType
        {
            Id = jobTypeId,
            Name = "Old Job Type",
            Description = "Old Description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var updateDto = new UpdateJobTypeDto
        {
            Name = "Updated Job Type",
            Description = "Updated Description"
        };

        _jobTypeRepositoryMock
            .Setup(repo => repo.GetByIdAsync(jobTypeId))
            .ReturnsAsync(existingJobType);

        _jobTypeRepositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<JobType>()))
            .ReturnsAsync(jobTypeId);

        // Act
        var result = await _service.UpdateAsync(jobTypeId, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updateDto.Name, result.Name);
        Assert.Equal(updateDto.Description, result.Description);
    }

    [Fact]
    public async Task UpdateAsync_WhenJobTypeDoesNotExist_ReturnsNull()
    {
        // Arrange
        var jobTypeId = Guid.NewGuid();
        var updateDto = new UpdateJobTypeDto
        {
            Name = "Updated Job Type",
            Description = "Updated Description"
        };

        _jobTypeRepositoryMock
            .Setup(repo => repo.GetByIdAsync(jobTypeId))
            .ReturnsAsync((JobType)null);

        // Act
        var result = await _service.UpdateAsync(jobTypeId, updateDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_CallsRepositoryDelete()
    {
        // Arrange
        var jobTypeId = Guid.NewGuid();

        // Act
        await _service.DeleteAsync(jobTypeId);

        // Assert
        _jobTypeRepositoryMock.Verify(repo => repo.DeleteAsync(jobTypeId), Times.Once);
    }

    [Fact]
    public async Task CreateBulkAsync_ValidData_CreatesAndReturnsJobTypeDtos()
    {
        // Arrange
        var createBulkDto = new CreateJobTypeBulkDto
        {
            JobTypes = new List<CreateJobTypeDto>
            {
                new() { Name = "Job Type 1", Description = "Description 1" },
                new() { Name = "Job Type 2", Description = "Description 2" },
                new() { Name = "Job Type 3", Description = "Description 3" }
            }
        };

        _jobTypeRepositoryMock
            .Setup(repo => repo.CreateBulkAsync(It.IsAny<IEnumerable<JobType>>()))
            .ReturnsAsync(new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() });

        // Act
        var result = await _service.CreateBulkAsync(createBulkDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        Assert.Contains(result, jt => jt.Name == "Job Type 1" && jt.Description == "Description 1");
        Assert.Contains(result, jt => jt.Name == "Job Type 2" && jt.Description == "Description 2");
        Assert.Contains(result, jt => jt.Name == "Job Type 3" && jt.Description == "Description 3");
    }

    [Fact]
    public async Task CreateBulkAsync_EmptyList_ReturnsEmptyCollection()
    {
        // Arrange
        var createBulkDto = new CreateJobTypeBulkDto
        {
            JobTypes = new List<CreateJobTypeDto>()
        };

        _jobTypeRepositoryMock
            .Setup(repo => repo.CreateBulkAsync(It.IsAny<IEnumerable<JobType>>()))
            .ReturnsAsync(new List<Guid>());

        // Act
        var result = await _service.CreateBulkAsync(createBulkDto);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
} 