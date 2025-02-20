using FluentValidation.TestHelper;
using Moq;
using Users.Application.Abstractions;
using Users.Application.Models;
using Users.Application.Repositories;
using Users.Application.Validators;

namespace Users.Tests.Application.Validators;

public class UserValidatorTests
{
    private readonly UserValidator _validator;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IDateTime> _mockDateTime;

    public UserValidatorTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockDateTime = new Mock<IDateTime>();

        _validator = new UserValidator(_mockUserRepository.Object, _mockDateTime.Object);
    }


    [Fact]
    public async Task Should_Pass_When_User_Is_Valid()
    {
        // Arrange
        var user = GetValidUser();
        _mockDateTime.Setup(d => d.UtcNow).Returns(new DateTime(2024, 1, 1));
        _mockUserRepository.Setup(r => r.GetByEmailAsync(user.EmailAddress.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);

        // Act
        var result = await _validator.TestValidateAsync(user);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_Fail_When_FirstName_Is_Empty()
    {
        var user = GetValidUser();
        user.PersonalData.FirstName = "";

        var result = await _validator.TestValidateAsync(user);

        result.ShouldHaveValidationErrorFor(x => x.PersonalData.FirstName)
            .WithErrorMessage("FirstName is required");
    }

    [Fact]
    public async Task Should_Fail_When_FirstName_Exceeds_MaxLength()
    {
        var user = GetValidUser();
        user.PersonalData.FirstName = new string('A', 129);

        var result = await _validator.TestValidateAsync(user);

        result.ShouldHaveValidationErrorFor(x => x.PersonalData.FirstName)
            .WithErrorMessage("Maximum length is 128 characters.");
    }

    [Fact]
    public async Task Should_Fail_When_LastName_Exceeds_MaxLength()
    {
        var user = GetValidUser();
        user.PersonalData.LastName = new string('A', 129);

        var result = await _validator.TestValidateAsync(user);

        result.ShouldHaveValidationErrorFor(x => x.PersonalData.LastName)
            .WithErrorMessage("Maximum length is 128 characters.");
    }

    [Fact]
    public async Task Should_Fail_When_Email_Is_Empty()
    {
        var user = GetValidUser();
        user.EmailAddress.Email = "";

        var result = await _validator.TestValidateAsync(user);

        result.ShouldHaveValidationErrorFor(x => x.EmailAddress.Email)
            .WithErrorMessage("Email is required.");
    }

    [Fact]
    public async Task Should_Fail_When_Email_Format_Is_Invalid()
    {
        var user = GetValidUser();
        user.EmailAddress.Email = "invalid-email";

        var result = await _validator.TestValidateAsync(user);

        result.ShouldHaveValidationErrorFor(x => x.EmailAddress.Email)
            .WithErrorMessage("Invalid email format.");
    }

    [Fact]
    public async Task Should_Fail_When_Email_Is_Not_Unique()
    {
        var user = GetValidUser();
        _mockUserRepository.Setup(r => r.GetByEmailAsync(user.EmailAddress.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User
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
            });

        var result = await _validator.TestValidateAsync(user);

        result.ShouldHaveValidationErrorFor(x => x.EmailAddress.Email)
            .WithErrorMessage("Email already registered.");
    }

    [Fact]
    public async Task Should_Fail_When_DateOfBirth_Is_Empty()
    {
        var user = GetValidUser();
        user.PersonalData.DateOfBirth = default;

        var result = await _validator.TestValidateAsync(user);

        result.ShouldHaveValidationErrorFor(x => x.PersonalData.DateOfBirth)
            .WithErrorMessage("DateOfBirth is required");
    }

    [Fact]
    public async Task Should_Fail_When_User_Is_Under_18()
    {
        var user = GetValidUser();
        user.PersonalData.DateOfBirth = new DateTime(2010, 1, 1);
        _mockDateTime.Setup(d => d.UtcNow).Returns(new DateTime(2024, 1, 1));

        var result = await _validator.TestValidateAsync(user);

        result.ShouldHaveValidationErrorFor(x => x.PersonalData.DateOfBirth)
            .WithErrorMessage("User should be over 18 years old.");
    }

    [Fact]
    public async Task Should_Fail_When_PhoneNumber_Is_Empty()
    {
        var user = GetValidUser();
        user.PersonalData.PhoneNumber = "";

        var result = await _validator.TestValidateAsync(user);

        result.ShouldHaveValidationErrorFor(x => x.PersonalData.PhoneNumber)
            .WithErrorMessage("Phone number is required.");
    }

    [Fact]
    public async Task Should_Fail_When_PhoneNumber_Is_Invalid()
    {
        var user = GetValidUser();
        user.PersonalData.PhoneNumber = "12345";

        var result = await _validator.TestValidateAsync(user);

        result.ShouldHaveValidationErrorFor(x => x.PersonalData.PhoneNumber)
            .WithErrorMessage("Phone number must contain only digits and be 10 characters long.");
    }

    [Fact]
    public async Task Should_Fail_When_PhoneNumber_Has_NonDigit_Characters()
    {
        var user = GetValidUser();
        user.PersonalData.PhoneNumber = "123456789A";

        var result = await _validator.TestValidateAsync(user);

        result.ShouldHaveValidationErrorFor(x => x.PersonalData.PhoneNumber)
            .WithErrorMessage("Phone number must contain only digits and be 10 characters long.");
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
}