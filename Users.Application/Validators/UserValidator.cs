using FluentValidation;
using System;
using Users.Application.Abstractions;
using Users.Application.Models;
using Users.Application.Repositories;

namespace Users.Application.Validators;

public class UserValidator : AbstractValidator<User>
{
    private readonly IUserRepository _userRepository;
    private readonly IDateTime _dateTime;

    public UserValidator(IUserRepository userRepository, IDateTime systemDateTime)
    {
        _userRepository = userRepository;
        _dateTime = systemDateTime;

        RuleFor(x => x.PersonalData.FirstName)
            .NotEmpty()
            .WithMessage("FirstName is required")
            .MaximumLength(128)
            .WithMessage("Maximum length is 128 characters.");

        RuleFor(x => x.PersonalData.LastName)
            .MaximumLength(128)
            .WithMessage("Maximum length is 128 characters."); ;

        RuleFor(x => x.EmailAddress.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")
            .WithMessage("Invalid email format.")
            .MustAsync(BeUnique).WithMessage("Email already registered.");

        RuleFor(x => x.PersonalData.DateOfBirth)
            .NotEmpty()
            .WithMessage("DateOfBirth is required")
            .Must(ValidateAge)
            .WithMessage("User should be over 18 years old.");

        RuleFor(x => x.PersonalData.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required.")
            .Matches(@"^\d{10}$")
            .WithMessage("Phone number must contain only digits and be 10 characters long.");
    }

    private bool ValidateAge(DateTime dateOfBirth)
    {
        int age = _dateTime.UtcNow.Year - dateOfBirth.Year;

        if (_dateTime.UtcNow < dateOfBirth.AddYears(age))
        {
            age--;
        }

        return age >= 18;
    }

    private async Task<bool> BeUnique(string email, CancellationToken token = default)
    {
        var user = await _userRepository.GetByEmailAsync(email, token);

        if (user is not null)
        {
            return false;
        }

        return true;
    }
}
