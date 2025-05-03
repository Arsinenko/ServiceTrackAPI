using AuthApp.domain.Entities;
using Xunit;

namespace ServiceTrack.Tests.Domain;

public class UserTests
{
    [Fact]
    public void User_WhenCreated_ShouldHaveCorrectInitialState()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@example.com";
        var roleId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        // Act
        var user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            RoleId = roleId,
            CreatedAt = createdAt
        };

        // Assert
        Assert.Equal(firstName, user.FirstName);
        Assert.Equal(lastName, user.LastName);
        Assert.Equal(email, user.Email);
        Assert.Equal(roleId, user.RoleId);
        Assert.True(user.IsAlive);
        Assert.Equal(createdAt, user.CreatedAt);
        Assert.Null(user.LastLoginAt);
        Assert.Null(user.UpdatedAt);
    }

    [Fact]
    public void User_WhenUpdated_ShouldUpdateUpdatedAt()
    {
        // Arrange
        var user = new User
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var updatedAt = DateTime.UtcNow;
        user.FirstName = "Jane";
        user.UpdatedAt = updatedAt;

        // Assert
        Assert.Equal("Jane", user.FirstName);
        Assert.Equal(updatedAt, user.UpdatedAt);
    }

    [Fact]
    public void User_WhenLoggedIn_ShouldUpdateLastLoginAt()
    {
        // Arrange
        var user = new User
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var lastLoginAt = DateTime.UtcNow;
        user.LastLoginAt = lastLoginAt;

        // Assert
        Assert.Equal(lastLoginAt, user.LastLoginAt);
    }

    [Fact]
    public void User_WhenDeactivated_ShouldSetIsAliveToFalse()
    {
        // Arrange
        var user = new User
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        user.IsAlive = false;

        // Assert
        Assert.False(user.IsAlive);
    }
} 