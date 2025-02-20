using FluentValidation;
using Moq;
using Users.Application.Abstractions;
using Users.Application.Models;
using Users.Application.Repositories;
using Users.Application.Services;
using Users.Application.Validators;

namespace Users.Tests.Application.Services;

public class UserServiceIntegrationTests
{
    private readonly UserService _service;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IDateTime> _dateTimeMock;
    private readonly IValidator<GetAllUsersOptions> _realUserOptionsValidator;
    private readonly IValidator<User> _realUserValidator;

    public UserServiceIntegrationTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();

        _dateTimeMock = new Mock<IDateTime>();
        _dateTimeMock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);

        _realUserOptionsValidator = new GetAllUsersOptionsValidator();
        _realUserValidator = new UserValidator(_mockUserRepository.Object, _dateTimeMock.Object);

        _service = new UserService(_mockUserRepository.Object, _realUserValidator, _realUserOptionsValidator);
    }

    [Fact]
    public async Task CreateAsync_Should_Create_User_When_Valid()
    {
        // Arrange
        var user = GetValidUser();

        var validationResult = await _realUserValidator.ValidateAsync(user);
        Assert.True(validationResult.IsValid);

        _mockUserRepository.Setup(r => r.CreateAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.CreateAsync(user);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteByIdAsync_Should_Return_True_When_User_Deleted()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.DeleteByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteByIdAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_Users_When_Valid()
    {
        // Arrange
        var options = GetValidOptions();
        var validationResult = await _realUserOptionsValidator.ValidateAsync(options);
        Assert.True(validationResult.IsValid);

        var users = new List<User> { GetValidUser() };
        _mockUserRepository.Setup(r => r.GetAllAsync(options, It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        // Act
        var result = await _service.GetAllAsync(options);

        // Assert
        Assert.Equal(users, result);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_User_When_Found()
    {
        // Arrange
        var user = GetValidUser();
        _mockUserRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.Equal(user, result);
    }

    [Fact]
    public async Task GetCountAsync_Should_Return_Count()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetCountAsync(It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(10);

        // Act
        var result = await _service.GetCountAsync(null);

        // Assert
        Assert.Equal(10, result);
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_User_When_Valid_And_Exists()
    {
        // Arrange
        var user = GetValidUser();
        var validationResult = await _realUserValidator.ValidateAsync(user);
        Assert.True(validationResult.IsValid);

        _mockUserRepository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockUserRepository.Setup(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(true));

        // Act
        var result = await _service.UpdateAsync(user);

        // Assert
        Assert.Equal(user, result);
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_ValidationException_When_User_Invalid()
    {
        // Arrange
        var user = GetValidUser();
        user.PersonalData.FirstName = "";
        var validationResult = await _realUserValidator.ValidateAsync(user);
        Assert.False(validationResult.IsValid);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(user));
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_User_Not_Found()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_Null_When_User_Does_Not_Exist()
    {
        // Arrange
        var user = GetValidUser();
        _mockUserRepository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);

        // Act
        var result = await _service.UpdateAsync(user);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteByIdAsync_Should_Return_False_When_User_Not_Found()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.DeleteByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _service.DeleteByIdAsync(1);

        // Assert
        Assert.False(result);
    }

    private static User GetValidUser()
    {
        return new User
        {
            Id = 1,
            PersonalData = new PersonalData
            {
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "1234567890",
                DateOfBirth = new DateTime(1990, 1, 1)
            },
            EmailAddress = new EmailAddress
            {
                Email = "john.doe@example.com"
            }
        };
    }

    private static GetAllUsersOptions GetValidOptions()
    {
        return new GetAllUsersOptions
        {
            Date = new DateTime(2000, 1, 1),
            Page = 1,
            PageSize = 10
        };
    }
}