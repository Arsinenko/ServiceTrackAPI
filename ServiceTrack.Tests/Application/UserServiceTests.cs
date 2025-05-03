using AuthApp.application.DTOs;
using AuthApp.application.Interfaces;
using AuthApp.application.Services;
using AuthApp.domain.Entities;
using Moq;
using Xunit;

namespace ServiceTrack.Tests.Application;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _service = new UserService(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserExists_ReturnsUserDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        var updatedAt = DateTime.UtcNow;
        var user = new User
        {
            Id = userId,
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            PasswordHash = "hashed_password",
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            Role = new Role { Id = Guid.NewGuid(), Name = "Test Role", Description = "Test Description" }
        };

        _userRepositoryMock
            .Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _service.GetByIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal("test@example.com", result.Email);
        Assert.Equal("Test", result.FirstName);
        Assert.Equal("User", result.LastName);
        Assert.Equal(createdAt.ToLocalTime(), result.CreatedAt);
        Assert.Equal(updatedAt.ToLocalTime(), result.UpdatedAt);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepositoryMock
            .Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync((User)null);

        // Act
        var result = await _service.GetByIdAsync(userId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenUserExists_ReturnsUserDto()
    {
        // Arrange
        var email = "test@example.com";
        var roleId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        var updatedAt = DateTime.UtcNow;
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "User",
            Email = email,
            PasswordHash = "hashed_password",
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            Role = new Role { Id = Guid.NewGuid(), Name = "Test Role", Description = "Test Description" }
        };

        _userRepositoryMock
            .Setup(repo => repo.GetByEmailAsync(email))
            .ReturnsAsync(user);

        // Act
        var result = await _service.GetByEmailAsync(email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
        Assert.Equal(user.FirstName, result.FirstName);
        Assert.Equal(user.LastName, result.LastName);
        Assert.NotNull(result.Role);
        Assert.Equal(user.Role.Name, result.Role.Name);
        Assert.Equal(user.Role.Description, result.Role.Description);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        // Arrange
        var email = "nonexistent@example.com";
        _userRepositoryMock
            .Setup(repo => repo.GetByEmailAsync(email))
            .ReturnsAsync((User)null);

        // Act
        var result = await _service.GetByEmailAsync(email);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllUsers()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        var updatedAt = DateTime.UtcNow;
        var users = new List<User>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Email = "user1@example.com",
                FirstName = "John",
                LastName = "Doe",
                RoleId = roleId,
                Role = new Role
                {
                    Id = roleId,
                    Name = "Test Role",
                    Description = "Test Description",
                    CreatedAt = createdAt,
                    UpdatedAt = updatedAt,
                    Users = new List<User>()
                },
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            },
            new()
            {
                Id = Guid.NewGuid(),
                Email = "user2@example.com",
                FirstName = "Jane",
                LastName = "Smith",
                RoleId = roleId,
                Role = new Role
                {
                    Id = roleId,
                    Name = "Test Role",
                    Description = "Test Description",
                    CreatedAt = createdAt,
                    UpdatedAt = updatedAt,
                    Users = new List<User>()
                },
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            }
        };

        _userRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(users);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, u => u.Email == "user1@example.com");
        Assert.Contains(result, u => u.Email == "user2@example.com");
    }

    [Fact]
    public async Task UpdateAsync_WhenUserExists_UpdatesAndReturnsUserDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        var updatedAt = DateTime.UtcNow;
        var existingUser = new User
        {
            Id = userId,
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            PasswordHash = "hashed_password",
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            Role = new Role { Id = Guid.NewGuid(), Name = "Test Role", Description = "Test Description" }
        };

        var updateDto = new UpdateUserDto
        {
            FirstName = "New",
            LastName = "Name",
            Email = "new@example.com",
            IsAlive = true
        };

        _userRepositoryMock
            .Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        _userRepositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync(userId);

        // Act
        var result = await _service.UpdateAsync(userId, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updateDto.FirstName, result.FirstName);
        Assert.Equal(updateDto.LastName, result.LastName);
        Assert.Equal(updateDto.Email, result.Email);
        Assert.Equal(updateDto.IsAlive, result.IsAlive);
    }

    [Fact]
    public async Task UpdateAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var updateDto = new UpdateUserDto
        {
            FirstName = "New",
            LastName = "Name",
            Email = "new@example.com",
            IsAlive = true
        };

        _userRepositoryMock
            .Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync((User)null);

        // Act
        var result = await _service.UpdateAsync(userId, updateDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenUserExists_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepositoryMock
            .Setup(repo => repo.SoftDeleteAsync(userId))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteAsync(userId);

        // Assert
        Assert.True(result);
        _userRepositoryMock.Verify(repo => repo.SoftDeleteAsync(userId), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenUserDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepositoryMock
            .Setup(repo => repo.SoftDeleteAsync(userId))
            .ReturnsAsync(false);

        // Act
        var result = await _service.DeleteAsync(userId);

        // Assert
        Assert.False(result);
        _userRepositoryMock.Verify(repo => repo.SoftDeleteAsync(userId), Times.Once);
    }
} 