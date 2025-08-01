using AuthApp.application.DTOs;
using AuthApp.application.Exceptions;
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
    public async Task UpdateAsync_WhenRoleDoesNotExist_ThrowsRoleNotFoundException()
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

        // Act & Assert
        await Assert.ThrowsAsync<RoleNotFoundException>(() => _service.UpdateAsync(roleId, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_CallsRepositoryDelete()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var role = new Role
        {
            Id = roleId,
            Name = "Test Role",
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow,
            Users = new List<User>()
        };

        _roleRepositoryMock
            .Setup(repo => repo.GetByIdAsync(roleId))
            .ReturnsAsync(role);

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
            Roles = new List<CreateRoleDto>
            {
                new() { Name = "Role 1", Description = "Description 1" },
                new() { Name = "Role 2", Description = "Description 2" },
                new() { Name = "Role 3", Description = "Description 3" }
            }
        };

        // Mock GetAllAsync to return empty list (no existing roles)
        _roleRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(new List<Role>());

        _roleRepositoryMock
            .Setup(repo => repo.CreateBulkAsync(It.IsAny<IEnumerable<Role>>()))
            .ReturnsAsync(new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() });

        // Act
        var result = await _service.CreateBulkAsync(createBulkDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.CreatedRoles.Count());
        Assert.Empty(result.FailedRoles);
        Assert.Empty(result.FailureReasons);
        Assert.Contains(result.CreatedRoles, r => r.Name == "Role 1" && r.Description == "Description 1");
        Assert.Contains(result.CreatedRoles, r => r.Name == "Role 2" && r.Description == "Description 2");
        Assert.Contains(result.CreatedRoles, r => r.Name == "Role 3" && r.Description == "Description 3");
    }

    [Fact]
    public async Task CreateBulkAsync_EmptyList_ReturnsEmptyCollection()
    {
        // Arrange
        var createBulkDto = new CreateRoleBulkDto { Roles = new List<CreateRoleDto>() };

        // Act
        var result = await _service.CreateBulkAsync(createBulkDto);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.CreatedRoles);
        Assert.Empty(result.FailedRoles);
        Assert.Empty(result.FailureReasons);
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

    [Fact]
    public async Task CreateAsync_WithExistingName_ThrowsRoleNameAlreadyExistsException()
    {
        // Arrange
        var createDto = new CreateRoleDto
        {
            Name = "Existing Role",
            Description = "Test Description"
        };

        var existingRole = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Existing Role",
            Description = "Existing Description",
            CreatedAt = DateTime.UtcNow
        };

        _roleRepositoryMock
            .Setup(repo => repo.GetByNameAsync(createDto.Name))
            .ReturnsAsync(existingRole);

        // Act & Assert
        await Assert.ThrowsAsync<RoleNameAlreadyExistsException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task UpdateAsync_WithExistingName_ThrowsRoleNameAlreadyExistsException()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var existingRole = new Role
        {
            Id = roleId,
            Name = "Old Role",
            Description = "Old Description",
            CreatedAt = DateTime.UtcNow
        };

        var updateDto = new UpdateRoleDto
        {
            Name = "Existing Role",
            Description = "Updated Description"
        };

        var otherExistingRole = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Existing Role",
            Description = "Other Description",
            CreatedAt = DateTime.UtcNow
        };

        _roleRepositoryMock
            .Setup(repo => repo.GetByIdAsync(roleId))
            .ReturnsAsync(existingRole);

        _roleRepositoryMock
            .Setup(repo => repo.GetByNameAsync(updateDto.Name))
            .ReturnsAsync(otherExistingRole);

        // Act & Assert
        await Assert.ThrowsAsync<RoleNameAlreadyExistsException>(() => _service.UpdateAsync(roleId, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_WhenRoleHasUsers_ThrowsRoleInUseException()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var role = new Role
        {
            Id = roleId,
            Name = "Test Role",
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow,
            Users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Email = "test@example.com" }
            }
        };

        _roleRepositoryMock
            .Setup(repo => repo.GetByIdAsync(roleId))
            .ReturnsAsync(role);

        // Act & Assert
        await Assert.ThrowsAsync<RoleInUseException>(() => _service.DeleteAsync(roleId));
    }

    [Theory]
    [InlineData("", "Description")]
    [InlineData(null, "Description")]
    [InlineData("Role", "")]
    [InlineData("Role", null)]
    [InlineData("ThisIsAVeryLongRoleNameThatExceedsTheMaximumAllowedLengthOfFiftyCharacters", "Description")]
    [InlineData("Role", "ThisIsAVeryLongDescriptionThatExceedsTheMaximumAllowedLengthOfTwoHundredCharactersThisIsAVeryLongDescriptionThatExceedsTheMaximumAllowedLengthOfTwoHundredCharactersThisIsAVeryLongDescriptionThatExceedsTheMaximumAllowedLengthOfTwoHundredCharacters")]
    public async Task CreateAsync_WithInvalidData_ThrowsRoleValidationException(string name, string description)
    {
        // Arrange
        var createDto = new CreateRoleDto
        {
            Name = name,
            Description = description
        };

        // Act & Assert
        await Assert.ThrowsAsync<RoleValidationException>(() => _service.CreateAsync(createDto));
    }

    [Theory]
    [InlineData("", "Description")]
    [InlineData(null, "Description")]
    [InlineData("Role", "")]
    [InlineData("Role", null)]
    [InlineData("ThisIsAVeryLongRoleNameThatExceedsTheMaximumAllowedLengthOfFiftyCharacters", "Description")]
    [InlineData("Role", "ThisIsAVeryLongDescriptionThatExceedsTheMaximumAllowedLengthOfTwoHundredCharactersThisIsAVeryLongDescriptionThatExceedsTheMaximumAllowedLengthOfTwoHundredCharactersThisIsAVeryLongDescriptionThatExceedsTheMaximumAllowedLengthOfTwoHundredCharacters")]
    public async Task UpdateAsync_WithInvalidData_ThrowsRoleValidationException(string name, string description)
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var existingRole = new Role
        {
            Id = roleId,
            Name = "Old Role",
            Description = "Old Description",
            CreatedAt = DateTime.UtcNow
        };

        var updateDto = new UpdateRoleDto
        {
            Name = name,
            Description = description
        };

        _roleRepositoryMock
            .Setup(repo => repo.GetByIdAsync(roleId))
            .ReturnsAsync(existingRole);

        // Act & Assert
        await Assert.ThrowsAsync<RoleValidationException>(() => _service.UpdateAsync(roleId, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_WhenRoleDoesNotExist_ThrowsRoleNotFoundException()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        _roleRepositoryMock
            .Setup(repo => repo.GetByIdAsync(roleId))
            .ReturnsAsync((Role)null);

        // Act & Assert
        await Assert.ThrowsAsync<RoleNotFoundException>(() => _service.DeleteAsync(roleId));
    }

    [Fact]
    public async Task DeleteBulkAsync_WhenAllRolesExistAndNotInUse_DeletesAllRoles()
    {
        // Arrange
        var roleIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var roles = new List<Role>
        {
            new()
            {
                Id = roleIds[0],
                Name = "Role 1",
                Description = "Description 1",
                CreatedAt = DateTime.UtcNow,
                Users = new List<User>()
            },
            new()
            {
                Id = roleIds[1],
                Name = "Role 2",
                Description = "Description 2",
                CreatedAt = DateTime.UtcNow,
                Users = new List<User>()
            }
        };

        _roleRepositoryMock
            .Setup(repo => repo.GetByIdsAsync(roleIds))
            .ReturnsAsync(roles);

        // Act
        var result = await _service.DeleteBulkAsync(roleIds);

        // Assert
        Assert.Equal(2, result.DeletedRoles.Count);
        Assert.Empty(result.FailedRoleIds);
        Assert.Empty(result.FailureReasons);
        Assert.Contains(result.DeletedRoles, r => r.Name == "Role 1");
        Assert.Contains(result.DeletedRoles, r => r.Name == "Role 2");

        foreach (var roleId in roleIds)
        {
            _roleRepositoryMock.Verify(repo => repo.DeleteAsync(roleId), Times.Once);
        }
    }

    [Fact]
    public async Task DeleteBulkAsync_WhenSomeRolesDoNotExist_ReturnsPartialSuccess()
    {
        // Arrange
        var existingRoleId = Guid.NewGuid();
        var nonExistingRoleId = Guid.NewGuid();
        var roleIds = new List<Guid> { existingRoleId, nonExistingRoleId };
        
        var existingRole = new Role
        {
            Id = existingRoleId,
            Name = "Existing Role",
            Description = "Description",
            CreatedAt = DateTime.UtcNow,
            Users = new List<User>()
        };

        _roleRepositoryMock
            .Setup(repo => repo.GetByIdsAsync(roleIds))
            .ReturnsAsync(new List<Role> { existingRole });

        // Act
        var result = await _service.DeleteBulkAsync(roleIds);

        // Assert
        Assert.Single(result.DeletedRoles);
        Assert.Single(result.FailedRoleIds);
        Assert.Single(result.FailureReasons);
        Assert.Contains(result.DeletedRoles, r => r.Id == existingRoleId);
        Assert.Contains(result.FailedRoleIds, id => id == nonExistingRoleId);
        Assert.Contains(result.FailureReasons, reason => reason.Contains(nonExistingRoleId.ToString()));

        _roleRepositoryMock.Verify(repo => repo.DeleteAsync(existingRoleId), Times.Once);
        _roleRepositoryMock.Verify(repo => repo.DeleteAsync(nonExistingRoleId), Times.Never);
    }

    [Fact]
    public async Task DeleteBulkAsync_WhenSomeRolesAreInUse_ReturnsPartialSuccess()
    {
        // Arrange
        var availableRoleId = Guid.NewGuid();
        var inUseRoleId = Guid.NewGuid();
        var roleIds = new List<Guid> { availableRoleId, inUseRoleId };
        
        var roles = new List<Role>
        {
            new()
            {
                Id = availableRoleId,
                Name = "Available Role",
                Description = "Description",
                CreatedAt = DateTime.UtcNow,
                Users = new List<User>()
            },
            new()
            {
                Id = inUseRoleId,
                Name = "In Use Role",
                Description = "Description",
                CreatedAt = DateTime.UtcNow,
                Users = new List<User> { new() { Id = Guid.NewGuid(), Email = "test@example.com" } }
            }
        };

        _roleRepositoryMock
            .Setup(repo => repo.GetByIdsAsync(roleIds))
            .ReturnsAsync(roles);

        // Act
        var result = await _service.DeleteBulkAsync(roleIds);

        // Assert
        Assert.Single(result.DeletedRoles);
        Assert.Single(result.FailedRoleIds);
        Assert.Single(result.FailureReasons);
        Assert.Contains(result.DeletedRoles, r => r.Id == availableRoleId);
        Assert.Contains(result.FailedRoleIds, id => id == inUseRoleId);
        Assert.Contains(result.FailureReasons, reason => reason.Contains("assigned to 1 users"));

        _roleRepositoryMock.Verify(repo => repo.DeleteAsync(availableRoleId), Times.Once);
        _roleRepositoryMock.Verify(repo => repo.DeleteAsync(inUseRoleId), Times.Never);
    }

    [Fact]
    public async Task DeleteBulkAsync_WhenAllRolesAreInUse_ReturnsAllFailed()
    {
        // Arrange
        var roleIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var roles = new List<Role>
        {
            new()
            {
                Id = roleIds[0],
                Name = "Role 1",
                Description = "Description 1",
                CreatedAt = DateTime.UtcNow,
                Users = new List<User> { new() { Id = Guid.NewGuid(), Email = "user1@example.com" } }
            },
            new()
            {
                Id = roleIds[1],
                Name = "Role 2",
                Description = "Description 2",
                CreatedAt = DateTime.UtcNow,
                Users = new List<User> { new() { Id = Guid.NewGuid(), Email = "user2@example.com" } }
            }
        };

        _roleRepositoryMock
            .Setup(repo => repo.GetByIdsAsync(roleIds))
            .ReturnsAsync(roles);

        // Act
        var result = await _service.DeleteBulkAsync(roleIds);

        // Assert
        Assert.Empty(result.DeletedRoles);
        Assert.Equal(2, result.FailedRoleIds.Count);
        Assert.Equal(2, result.FailureReasons.Count);
        Assert.Contains(result.FailedRoleIds, id => id == roleIds[0]);
        Assert.Contains(result.FailedRoleIds, id => id == roleIds[1]);
        Assert.All(result.FailureReasons, reason => reason.Contains("assigned to 1 users"));

        foreach (var roleId in roleIds)
        {
            _roleRepositoryMock.Verify(repo => repo.DeleteAsync(roleId), Times.Never);
        }
    }

    [Fact]
    public async Task DeleteBulkAsync_WhenEmptyListProvided_ReturnsEmptyResult()
    {
        // Arrange
        var roleIds = new List<Guid>();

        // Act
        var result = await _service.DeleteBulkAsync(roleIds);

        // Assert
        Assert.Empty(result.DeletedRoles);
        Assert.Empty(result.FailedRoleIds);
        Assert.Empty(result.FailureReasons);

        _roleRepositoryMock.Verify(repo => repo.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()), Times.Never);
        _roleRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }
} 