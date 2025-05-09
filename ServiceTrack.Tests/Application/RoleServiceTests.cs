using AuthApp.application.DTOs;
using AuthApp.application.Interfaces;
using AuthApp.application.Services;
using AuthApp.domain.Entities;
using Moq;
using Xunit;

namespace ServiceTrack.Tests.Application;

public class RoleServiceTests
{
    private readonly Mock<IRoleRepository> _roleRepositoryMock;
    private readonly RoleService _service;

    public RoleServiceTests()
    {
        _roleRepositoryMock = new Mock<IRoleRepository>();
        _service = new RoleService(_roleRepositoryMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenRoleExists_ReturnsRoleDto()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var role = new Role
        {
            Id = roleId,
            Name = "Test Role",
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Users = new List<User>()
        };

        _roleRepositoryMock
            .Setup(repo => repo.GetByIdAsync(roleId))
            .ReturnsAsync(role);

        // Act
        var result = await _service.GetByIdAsync(roleId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(roleId, result.Id);
        Assert.Equal("Test Role", result.Name);
        Assert.Equal("Test Description", result.Description);
        Assert.Equal(role.CreatedAt.ToLocalTime(), result.CreatedAt);
        Assert.Equal(role.UpdatedAt?.ToLocalTime(), result.UpdatedAt);
    }

    [Fact]
    public async Task GetByIdAsync_WhenRoleDoesNotExist_ReturnsNull()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        _roleRepositoryMock
            .Setup(repo => repo.GetByIdAsync(roleId))
            .ReturnsAsync((Role)null);

        // Act
        var result = await _service.GetByIdAsync(roleId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNameAsync_WhenRoleExists_ReturnsRoleDto()
    {
        // Arrange
        var roleName = "Test Role";
        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = roleName,
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Users = new List<User>()
        };

        _roleRepositoryMock
            .Setup(repo => repo.GetByNameAsync(roleName))
            .ReturnsAsync(role);

        // Act
        var result = await _service.GetByNameAsync(roleName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(roleName, result.Name);
        Assert.Equal("Test Description", result.Description);
        Assert.Equal(role.CreatedAt.ToLocalTime(), result.CreatedAt);
        Assert.Equal(role.UpdatedAt?.ToLocalTime(), result.UpdatedAt);
    }

    [Fact]
    public async Task GetByNameAsync_WhenRoleDoesNotExist_ReturnsNull()
    {
        // Arrange
        var roleName = "Non-existent Role";
        _roleRepositoryMock
            .Setup(repo => repo.GetByNameAsync(roleName))
            .ReturnsAsync((Role)null);

        // Act
        var result = await _service.GetByNameAsync(roleName);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllRoles()
    {
        // Arrange
        var roles = new List<Role>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Role 1",
                Description = "Description 1",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Users = new List<User>()
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Role 2",
                Description = "Description 2",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Users = new List<User>()
            }
        };

        _roleRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(roles);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, r => r.Name == "Role 1");
        Assert.Contains(result, r => r.Name == "Role 2");
    }

    [Fact]
    public async Task CreateAsync_ValidData_CreatesAndReturnsRoleDto()
    {
        // Arrange
        var createDto = new CreateRoleDto
        {
            Name = "New Role",
            Description = "New Description"
        };

        _roleRepositoryMock
            .Setup(repo => repo.CreateAsync(It.IsAny<Role>()))
            .ReturnsAsync(Guid.NewGuid());

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createDto.Name, result.Name);
        Assert.Equal(createDto.Description, result.Description);
    }

    [Fact]
    public async Task UpdateAsync_WhenRoleExists_UpdatesAndReturnsRoleDto()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var existingRole = new Role
        {
            Id = roleId,
            Name = "Old Role",
            Description = "Old Description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Users = new List<User>()
        };

        var updateDto = new UpdateRoleDto
        {
            Name = "Updated Role",
            Description = "Updated Description"
        };

        _roleRepositoryMock
            .Setup(repo => repo.GetByIdAsync(roleId))
            .ReturnsAsync(existingRole);

        _roleRepositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Role>()))
            .ReturnsAsync(existingRole);

        // Act
        var result = await _service.UpdateAsync(roleId, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updateDto.Name, result.Name);
        Assert.Equal(updateDto.Description, result.Description);
    }

    [Fact]
    public async Task UpdateAsync_WhenRoleDoesNotExist_ReturnsNull()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var updateDto = new UpdateRoleDto
        {
            Name = "Updated Role",
            Description = "Updated Description"
        };

        _roleRepositoryMock
            .Setup(repo => repo.GetByIdAsync(roleId))
            .ReturnsAsync((Role)null);

        // Act
        var result = await _service.UpdateAsync(roleId, updateDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_CallsRepositoryDelete()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        // Act
        await _service.DeleteAsync(roleId);

        // Assert
        _roleRepositoryMock.Verify(repo => repo.DeleteAsync(roleId), Times.Once);
    }

    [Fact]
    public async Task CreateBulkAsync_ValidData_CreatesAndReturnsRoleDtos()
    {
        // Arrange
        var createBulkDto = new CreateRoleBulkDto
        {
            Roles = new List<RoleDto>
            {
                new() { Name = "Role 1", Description = "Description 1" },
                new() { Name = "Role 2", Description = "Description 2" },
                new() { Name = "Role 3", Description = "Description 3" }
            }
        };

        _roleRepositoryMock
            .Setup(repo => repo.CreateBulkAsync(It.IsAny<IEnumerable<Role>>()))
            .ReturnsAsync(new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() });

        // Act
        var result = await _service.CreateBulkAsync(createBulkDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        Assert.Contains(result, r => r.Name == "Role 1" && r.Description == "Description 1");
        Assert.Contains(result, r => r.Name == "Role 2" && r.Description == "Description 2");
        Assert.Contains(result, r => r.Name == "Role 3" && r.Description == "Description 3");
    }

    [Fact]
    public async Task CreateBulkAsync_EmptyList_ReturnsEmptyCollection()
    {
        // Arrange
        var createBulkDto = new CreateRoleBulkDto { Roles = new List<RoleDto>() };

        // Act
        var result = await _service.CreateBulkAsync(createBulkDto);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task UpdateBulkAsync_ValidData_UpdatesAndReturnsRoleDtos()
    {
        // Arrange
        var updateBulkDto = new UpdateRoleBulkDto
        {
            Roles = new List<UpdateRoleBulkItemDto>
            {
                new() { Id = Guid.NewGuid(), Name = "Updated Role 1", Description = "Updated Description 1" },
                new() { Id = Guid.NewGuid(), Name = "Updated Role 2", Description = "Updated Description 2" }
            }
        };

        var updatedRoles = updateBulkDto.Roles.Select(dto => new Role
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        }).ToList();

        _roleRepositoryMock
            .Setup(repo => repo.UpdateBulkAsync(It.IsAny<IEnumerable<Role>>()))
            .ReturnsAsync(updatedRoles);

        // Act
        var result = await _service.UpdateBulkAsync(updateBulkDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.Name == "Updated Role 1" && r.Description == "Updated Description 1");
        Assert.Contains(result, r => r.Name == "Updated Role 2" && r.Description == "Updated Description 2");
    }
} 