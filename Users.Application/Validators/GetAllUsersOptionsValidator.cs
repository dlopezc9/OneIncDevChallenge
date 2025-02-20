using FluentValidation;
using Users.Application.Abstractions;
using Users.Application.Models;

namespace Users.Application.Validators;

public class GetAllUsersOptionsValidator : AbstractValidator<GetAllUsersOptions>
{
    public GetAllUsersOptionsValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("The minimum page is 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 25)
            .WithMessage("The size of the page must be between 1 and 25.");
    }
}
