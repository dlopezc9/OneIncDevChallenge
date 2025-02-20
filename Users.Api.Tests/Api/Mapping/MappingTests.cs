using FluentValidation.TestHelper;
using Moq;
using Users.Api.Mapping;
using Users.Application.Abstractions;
using Users.Application.Models;
using Users.Application.Repositories;
using Users.Application.Validators;
using Users.Contracts.Request;

namespace Users.Tests.Api.Mapping;

public class MappingTests
{
    private readonly UserValidator _validator;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IDateTime> _mockDateTime;

    public MappingTests()
    {
        _mockDateTime = new Mock<IDateTime>();
        _mockUserRepository = new Mock<IUserRepository>();
        _validator = new UserValidator(_mockUserRepository.Object, _mockDateTime.Object);

        ContractMapping.ConfigureDependencies(_mockDateTime.Object);
    }

    [Fact]
    public void MapToUser_ShouldMapCorrectly()
    {
        // Arrenge
        var request = new CreateUserRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            DateOfBirth = new DateTime(2000, 1, 1),
            PhoneNumber = "1234567890"
        };

        _mockDateTime.Setup(d => d.UtcNow).Returns(new DateTime(2024, 2, 19));

        // Act
        var result = request.MapToUser();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.Id);
        Assert.Equal("John", result.PersonalData.FirstName);
        Assert.Equal("Doe", result.PersonalData.LastName);
        Assert.Equal("john.doe@example.com", result.EmailAddress.Email);
        Assert.Equal(new DateTime(2000, 1, 1), result.PersonalData.DateOfBirth);
    }

    [Fact]
    public void MapToResponse_ShouldMapCorrectly()
    {
        // Arrenge
        var user = new User
        {
            Id = 1,
            PersonalData = new PersonalData
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(2000, 1, 1),
                PhoneNumber = "1234567890"
            },
            EmailAddress = new EmailAddress
            {
                Email = "john.doe@example.com"
            }
        };

        _mockDateTime.Setup(d => d.UtcNow).Returns(new DateTime(2024, 2, 19));

        // Act
        var result = user.MapToResponse();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Doe", result.LastName);
        Assert.Equal(24, result.Age);
        Assert.Equal("john.doe@example.com", result.Email);
        Assert.Equal("1234567890", result.PhoneNumber);
    }

    [Fact]
    public void MapToResponse_Enumerable_ShouldMapCorrectly()
    {
        // Arrenge
        var users = new List<User>
        {
            new User
            {
                Id = 1,
                PersonalData = new PersonalData
                {
                    FirstName = "John",
                    LastName = "Doe",
                    DateOfBirth = new DateTime(2000, 1, 1),
                    PhoneNumber = "1234567890"
                },
                EmailAddress = new EmailAddress
                {
                    Email = "john.doe@example.com"
                }
            }
        };

        _mockDateTime.Setup(d => d.UtcNow).Returns(new DateTime(2024, 2, 19));

        // Act
        var result = users.MapToResponse(page: 1, pageSize: 10, userCount: 1);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Users);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(1, result.Total);
    }

    [Fact]
    public void MapToUser_UpdateRequest_ShouldMapCorrectly()
    {
        // Arrenge
        var request = new UpdateUserRequest
        {
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane.doe@example.com",
            DateOfBirth = new DateTime(1990, 5, 15),
            PhoneNumber = "9876543210"
        };

        _mockDateTime.Setup(d => d.UtcNow).Returns(new DateTime(2024, 2, 19));

        // Act
        var result = request.MapToUser(id: 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
        Assert.Equal("Jane", result.PersonalData.FirstName);
        Assert.Equal("Doe", result.PersonalData.LastName);
        Assert.Equal("jane.doe@example.com", result.EmailAddress.Email);
        Assert.Equal(new DateTime(1990, 5, 15), result.PersonalData.DateOfBirth);
    }

    [Fact]
    public void MapToOptions_ShouldMapCorrectly()
    {
        // Arrenge
        var request = new GetAllUsersRequest
        {
            Date = new DateTime(2023, 1, 1),
            Page = 2,
            PageSize = 20
        };

        // Act
        var result = request.MapToOptions();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(new DateTime(2023, 1, 1), result.Date);
        Assert.Equal(2, result.Page);
        Assert.Equal(20, result.PageSize);
    }

    [Fact]
    public void CalculateAge_ShouldReturnCorrectAge()
    {
        // Arrenge
        var dateOfBirth = new DateTime(2000, 1, 1);
        _mockDateTime.Setup(d => d.UtcNow).Returns(new DateTime(2024, 2, 19));

        var request = new CreateUserRequest
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test.user@example.com",
            DateOfBirth = dateOfBirth,
            PhoneNumber = "1234567890"
        };

        // Act
        var result = request.MapToUser();

        // Assert
        Assert.Equal(24, CalculateAgeIndirectly(result.PersonalData.DateOfBirth));
    }

    [Fact]
    public async Task Should_Fail_When_FirstName_Is_Empty()
    {
        // Arrenge
        var user = new User
        {
            Id = 0,
            PersonalData = new PersonalData { FirstName = "", LastName = "Doe", DateOfBirth = new DateTime(1996, 2, 11), PhoneNumber = "3173785850" },
            EmailAddress = new EmailAddress { Email = "test@example.com" }
        };

        // Act
        var result = await _validator.TestValidateAsync(user);

        // Assert
        result.ShouldHaveValidationErrorFor(u => u.PersonalData.FirstName)
            .WithErrorMessage("FirstName is required");
    }

    [Fact]
    public async Task Should_Fail_When_FirstName_Exceeds_Max_Length()
    {
        // Arrenge
        var user = new User
        {
            Id = 0,
            PersonalData = new PersonalData { FirstName = new string('A', 129), LastName = "Doe", DateOfBirth = new DateTime(1996, 2, 11), PhoneNumber = "3173785850" },
            EmailAddress = new EmailAddress { Email = "test@example.com" }
        };

        // Act
        var result = await _validator.TestValidateAsync(user);

        // Assert
        result.ShouldHaveValidationErrorFor(u => u.PersonalData.FirstName)
            .WithErrorMessage("Maximum length is 128 characters.");
    }

    [Fact]
    public async Task Should_Fail_When_LastName_Exceeds_Max_Length()
    {
        // Arrenge
        var user = new User
        {
            Id = 0,
            PersonalData = new PersonalData { FirstName = "Jhon", LastName = new string('B', 129), DateOfBirth = new DateTime(1996, 2, 11), PhoneNumber = "3173785850" },
            EmailAddress = new EmailAddress { Email = "test@example.com" }
        };

        // Act
        var result = await _validator.TestValidateAsync(user);

        // Assert
        result.ShouldHaveValidationErrorFor(u => u.PersonalData.LastName)
            .WithErrorMessage("Maximum length is 128 characters.");
    }

    [Fact]
    public async Task Should_Fail_When_Email_Is_Invalid()
    {
        // Arrenge
        var user = new User
        {
            Id = 0,
            PersonalData = new PersonalData { FirstName = "Jhon", LastName = "Doe", DateOfBirth = new DateTime(1996, 2, 11), PhoneNumber = "3173785850" },
            EmailAddress = new EmailAddress { Email = "invalid-email" }
        };

        // Act
        var result = await _validator.TestValidateAsync(user);

        //Assert
        result.ShouldHaveValidationErrorFor(u => u.EmailAddress.Email)
            .WithErrorMessage("Invalid email format.");
    }

    [Fact]
    public async Task Should_Fail_When_Email_Is_Not_Unique()
    {
        // Arrenge
        var user = new User
        {
            Id = 0,
            PersonalData = new PersonalData { FirstName = "Jhon", LastName = "Doe", DateOfBirth = new DateTime(1996, 2, 11), PhoneNumber = "3173785850" },
            EmailAddress = new EmailAddress { Email = "test@example.com" }
        };

        _mockUserRepository.Setup(r => r.GetByEmailAsync("test@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _validator.TestValidateAsync(user);

        // Assert
        result.ShouldHaveValidationErrorFor(u => u.EmailAddress.Email)
            .WithErrorMessage("Email already registered.");
    }

    [Fact]
    public async Task Should_Fail_When_DateOfBirth_Is_Empty()
    {
        // Arrenge
        var user = new User
        {
            Id = 0,
            PersonalData = new PersonalData { FirstName = "Jhon", LastName = "Doe", DateOfBirth = default, PhoneNumber = "3173785850" },
            EmailAddress = new EmailAddress { Email = "test@example.com" }
        };

        // Act
        var result = await _validator.TestValidateAsync(user);

        // Assert
        result.ShouldHaveValidationErrorFor(u => u.PersonalData.DateOfBirth)
            .WithErrorMessage("DateOfBirth is required");
    }

    [Fact]
    public async Task Should_Fail_When_User_Is_Under_18()
    {
        // Arrenge
        _mockDateTime.Setup(d => d.UtcNow).Returns(new DateTime(2024, 2, 19));

        var user = new User
        {
            Id = 0,
            PersonalData = new PersonalData { FirstName = "Jhon", LastName = "Doe", DateOfBirth = new DateTime(2010, 1, 1), PhoneNumber = "3173785850" },
            EmailAddress = new EmailAddress { Email = "test@example.com" }
        };

        // Act
        var result = await _validator.TestValidateAsync(user);

        // Assert
        result.ShouldHaveValidationErrorFor(u => u.PersonalData.DateOfBirth)
            .WithErrorMessage("User should be over 18 years old.");
    }

    [Fact]
    public async Task Should_Fail_When_PhoneNumber_Is_Invalid()
    {
        // Arrenge
        var user = new User
        {
            Id = 0,
            PersonalData = new PersonalData { FirstName = "Jhon", LastName = "Doe", DateOfBirth = new DateTime(1996, 2, 11), PhoneNumber = "35850" },
            EmailAddress = new EmailAddress { Email = "test@example.com" }
        };

        // Act
        var result = await _validator.TestValidateAsync(user);

        // Assert
        result.ShouldHaveValidationErrorFor(u => u.PersonalData.PhoneNumber)
            .WithErrorMessage("Phone number must contain only digits and be 10 characters long.");
    }

    [Fact]
    public async Task Should_Fail_When_PhoneNumber_Is_Empty()
    {
        // Arrenge
        var user = new User
        {
            Id = 0,
            PersonalData = new PersonalData { FirstName = "Jhon", LastName = "Doe", DateOfBirth = new DateTime(1996, 2, 11), PhoneNumber = "" },
            EmailAddress = new EmailAddress { Email = "test@example.com" }
        };

        // Act
        var result = await _validator.TestValidateAsync(user);

        // Assert
        result.ShouldHaveValidationErrorFor(u => u.PersonalData.PhoneNumber)
            .WithErrorMessage("Phone number is required.");
    }


    private int CalculateAgeIndirectly(DateTime dateOfBirth)
    {
        var age = _mockDateTime.Object.UtcNow.Year - dateOfBirth.Year;
        if (_mockDateTime.Object.UtcNow < dateOfBirth.AddYears(age))
        {
            age--;
        }
        return age;
    }
}
