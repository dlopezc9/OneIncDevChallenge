using Microsoft.AspNetCore.Mvc;
using Moq;
using Users.Api.Controllers;
using Users.Api.Mapping;
using Users.Application.Models;
using Users.Application.Services;
using Users.Contracts.Request;

namespace Users.Tests.Api.Controllers;

public class UsersControllerTests
{
    private readonly UsersController _controller;
    private readonly Mock<IUserService> _mockUserService;

    public UsersControllerTests()
    {
        _mockUserService = new Mock<IUserService>();
        _controller = new UsersController(_mockUserService.Object);
    }

    [Fact]
    public async Task Create_Should_Return_CreatedAtAction_When_Successful()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            PhoneNumber = "1234567890",
            DateOfBirth = new DateTime(1996, 1, 12)
        };

        var user = request.MapToUser();
        _mockUserService.Setup(s => s.CreateAsync(user, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(true));

        // Act
        var result = await _controller.Create(request, CancellationToken.None);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.Get), createdAtActionResult.ActionName);
    }

    [Fact]
    public async Task Get_Should_Return_Ok_When_User_Exists()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            PersonalData = new PersonalData
            {
                FirstName = "John",
                DateOfBirth = new DateTime(1994, 12, 2),
                LastName = "Doe",
                PhoneNumber = "1234567890"
            },
            EmailAddress = new EmailAddress
            {
                Email = "valid@test.com"
            }
        };
        _mockUserService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.Get(1, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetAll_Should_Return_Ok_When_Users_Exist()
    {
        // Arrange
        var request = new GetAllUsersRequest { Page = 1, PageSize = 10 };
        var users = new List<User> { new User
            {
                Id = 1,
                PersonalData = new PersonalData
                {
                    FirstName = "Updated",
                    DateOfBirth = new DateTime(1996, 2, 1),
                    PhoneNumber = "1234567890",
                    LastName = "Doe"
                },
                EmailAddress = new EmailAddress
                {
                    Email = "valid@test.com"
                }
            }
        };
        _mockUserService.Setup(s => s.GetAllAsync(It.IsAny<GetAllUsersOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        _mockUserService.Setup(s => s.GetCountAsync(It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _controller.GetAll(request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task Update_Should_Return_Ok_When_User_Updated()
    {
        // Arrange
        var request = new UpdateUserRequest { FirstName = "Updated", DateOfBirth = new DateTime(1996, 2, 1), Email = "valid@test.com", PhoneNumber = "1234567890" };
        var user = request.MapToUser(1);

        _mockUserService.Setup(s => s.UpdateAsync(It.IsAny<User?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.Update(1, request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task Delete_Should_Return_Ok_When_User_Deleted()
    {
        // Arrange
        _mockUserService.Setup(s => s.DeleteByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(1, CancellationToken.None);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Get_Should_Return_NotFound_When_User_Does_Not_Exist()
    {
        // Arrange
        _mockUserService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);

        // Act
        var result = await _controller.Get(1, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Update_Should_Return_NotFound_When_User_Does_Not_Exist()
    {
        // Arrange
        var request = new UpdateUserRequest { FirstName = "Updated", DateOfBirth = new DateTime(1996, 2, 1), Email = "valid@test.com", PhoneNumber = "1234567890" };
        var user = request.MapToUser(1);

        _mockUserService.Setup(s => s.UpdateAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);

        // Act
        var result = await _controller.Update(1, request, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_Should_Return_NotFound_When_User_Does_Not_Exist()
    {
        // Arrange
        _mockUserService.Setup(s => s.DeleteByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(1, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}