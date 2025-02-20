using FluentValidation.TestHelper;
using Users.Application.Models;
using Users.Application.Validators;

namespace Users.Tests.Application.Validators;
public class GetAllUsersOptionsValidatorTests
{
    private readonly GetAllUsersOptionsValidator _validator;

    public GetAllUsersOptionsValidatorTests()
    {
        _validator = new GetAllUsersOptionsValidator();
    }

    [Fact]
    public void Should_Pass_When_Options_Are_Valid()
    {
        // Arrange
        var date = DateTime.UtcNow;
        var options = new GetAllUsersOptions
        {
            Date = date.AddYears(-20),
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    // Page Validation
    [Fact]
    public void Should_Fail_When_Page_Is_Less_Than_1()
    {
        // Arrange
        var date = DateTime.UtcNow;
        var options = new GetAllUsersOptions
        {
            Date = date.AddYears(-20),
            Page = 0,
            PageSize = 10
        };

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Page)
            .WithErrorMessage("The minimum page is 1.");
    }

    [Fact]
    public void Should_Pass_When_Page_Is_Exactly_1()
    {
        // Arrange
        var date = DateTime.UtcNow;
        var options = new GetAllUsersOptions
        {
            Date = date.AddYears(-20),
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Page);
    }

    // PageSize Validation
    [Fact]
    public void Should_Fail_When_PageSize_Is_Less_Than_1()
    {
        // Arrange
        var date = DateTime.UtcNow;
        var options = new GetAllUsersOptions
        {
            Date = date.AddYears(-20),
            Page = 1,
            PageSize = 0
        };

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("The size of the page must be between 1 and 25.");
    }

    [Fact]
    public void Should_Fail_When_PageSize_Is_Greater_Than_25()
    {
        // Arrange
        var date = DateTime.UtcNow;
        var options = new GetAllUsersOptions
        {
            Date = date.AddYears(-20),
            Page = 1,
            PageSize = 26
        };

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("The size of the page must be between 1 and 25.");
    }

    [Fact]
    public void Should_Pass_When_PageSize_Is_Exactly_1()
    {
        // Arrange
        var date = DateTime.UtcNow;
        var options = new GetAllUsersOptions
        {
            Date = date.AddYears(-20),
            Page = 1,
            PageSize = 1
        };

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void Should_Pass_When_PageSize_Is_Exactly_25()
    {
        // Arrange
        var date = DateTime.UtcNow;
        var options = new GetAllUsersOptions
        {
            Date = date.AddYears(-20),
            Page = 1,
            PageSize = 25
        };

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
    }
}