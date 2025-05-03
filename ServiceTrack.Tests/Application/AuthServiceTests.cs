using AuthApp.application.DTOs;
using AuthApp.application.Interfaces;
using AuthApp.application.Services;
using AuthApp.domain.Entities;
using Moq;
using Xunit;

namespace ServiceTrack.Tests.Application;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IJwtGenerator> _jwtGeneratorMock;
    private readonly Mock<IRoleRepository> _roleRepositoryMock;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtGeneratorMock = new Mock<IJwtGenerator>();
        _roleRepositoryMock = new Mock<IRoleRepository>();
        
        _service = new AuthService(
            _userRepositoryMock.Object,
            _passwordHasherMock.Object,
            _jwtGeneratorMock.Object,
            _roleRepositoryMock.Object
        );
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_ReturnsSuccessResult()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var role = new Role { Id = roleId, Name = "User" };
        var registerDto = new RegisterUserDto
        {
            Email = "test@example.com",
            Password = "password123",
            FirstName = "John",
            LastName = "Doe",
            RoleId = roleId
        };

        _userRepositoryMock
            .Setup(repo => repo.GetByEmailAsync(registerDto.Email))
            .ReturnsAsync((User)null);

        _roleRepositoryMock
            .Setup(repo => repo.GetByIdAsync(roleId))
            .ReturnsAsync(role);

        _passwordHasherMock
            .Setup(hasher => hasher.HashPassword(registerDto.Password))
            .Returns("hashed_password");

        _jwtGeneratorMock
            .Setup(generator => generator.CreateToken(It.IsAny<User>()))
            .Returns("jwt_token");

        // Act
        var result = await _service.RegisterAsync(registerDto);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Registration success", result.Message);
        Assert.Equal(registerDto.Email, result.Email);
        Assert.Equal("jwt_token", result.Token);
        Assert.NotNull(result.User);
        Assert.Equal(registerDto.FirstName, result.User.FirstName);
        Assert.Equal(registerDto.LastName, result.User.LastName);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ReturnsFailureResult()
    {
        // Arrange
        var existingUser = new User
        {
            Email = "test@example.com",
            FirstName = "Existing",
            LastName = "User"
        };

        var registerDto = new RegisterUserDto
        {
            Email = "test@example.com",
            Password = "password123",
            FirstName = "John",
            LastName = "Doe",
            RoleId = Guid.NewGuid()
        };

        _userRepositoryMock
            .Setup(repo => repo.GetByEmailAsync(registerDto.Email))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _service.RegisterAsync(registerDto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Email already exists", result.Message);
    }

    [Fact]
    public async Task RegisterAsync_WithInvalidRole_ReturnsFailureResult()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var registerDto = new RegisterUserDto
        {
            Email = "test@example.com",
            Password = "password123",
            FirstName = "John",
            LastName = "Doe",
            RoleId = roleId
        };

        _userRepositoryMock
            .Setup(repo => repo.GetByEmailAsync(registerDto.Email))
            .ReturnsAsync((User)null);

        _roleRepositoryMock
            .Setup(repo => repo.GetByIdAsync(roleId))
            .ReturnsAsync((Role)null);

        // Act
        var result = await _service.RegisterAsync(registerDto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Specified role not found", result.Message);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsSuccessResult()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            PasswordHash = "hashed_password",
            FirstName = "John",
            LastName = "Doe"
        };

        var loginDto = new LoginUserDto
        {
            Email = "test@example.com",
            Password = "password123"
        };

        _userRepositoryMock
            .Setup(repo => repo.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(hasher => hasher.VerifyHashedPassword(user.PasswordHash, loginDto.Password))
            .Returns(true);

        _jwtGeneratorMock
            .Setup(generator => generator.CreateToken(user))
            .Returns("jwt_token");

        // Act
        var result = await _service.LoginAsync(loginDto);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Login success", result.Message);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal("jwt_token", result.Token);
        Assert.NotNull(result.User);
        Assert.Equal(user.FirstName, result.User.FirstName);
        Assert.Equal(user.LastName, result.User.LastName);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidEmail_ReturnsFailureResult()
    {
        // Arrange
        var loginDto = new LoginUserDto
        {
            Email = "nonexistent@example.com",
            Password = "password123"
        };

        _userRepositoryMock
            .Setup(repo => repo.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync((User)null);

        // Act
        var result = await _service.LoginAsync(loginDto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Email not exists", result.Message);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ReturnsFailureResult()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            PasswordHash = "hashed_password",
            FirstName = "John",
            LastName = "Doe"
        };

        var loginDto = new LoginUserDto
        {
            Email = "test@example.com",
            Password = "wrong_password"
        };

        _userRepositoryMock
            .Setup(repo => repo.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(hasher => hasher.VerifyHashedPassword(user.PasswordHash, loginDto.Password))
            .Returns(false);

        // Act
        var result = await _service.LoginAsync(loginDto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Invalid Email or password", result.Message);
    }
} 