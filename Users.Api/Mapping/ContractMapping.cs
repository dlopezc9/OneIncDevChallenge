using Users.Application.Abstractions;
using Users.Application.Models;
using Users.Contracts.Request;
using Users.Contracts.Responses;

namespace Users.Api.Mapping;

public static class ContractMapping
{
    public static IDateTime _dateTime { get; private set; } = new SystemDateTime();

    public static void ConfigureDependencies(IDateTime dateTime)
    {
        _dateTime = dateTime ?? throw new ArgumentNullException(nameof(dateTime));
    }

    public static User MapToUser(this CreateUserRequest request)
    {
        var age = CalculateAge(request.DateOfBirth);

        var personalData = new PersonalData
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = request.DateOfBirth
        };

        var email = new EmailAddress 
        { 
            Email = request.Email 
        };

        return new User
        {
            Id = 0,
            PersonalData = personalData,
            EmailAddress = email
        };
    }

    public static UserResponse MapToResponse(this User user)
    {
        var age = CalculateAge(user.PersonalData.DateOfBirth);

        return new UserResponse
        {
            FirstName = user.PersonalData.FirstName,
            LastName = user.PersonalData.LastName,
            Age = age,
            Email = user.EmailAddress.Email,
            DateOfBirth = user.PersonalData.DateOfBirth,
            PhoneNumber = user.PersonalData.PhoneNumber
        };
    }

    public static UsersResponse MapToResponse(this IEnumerable<User> users,
        int page, int pageSize,
        int userCount)
    {
        return new UsersResponse
        {
            Users = users.Select(MapToResponse),
            Page = page,
            PageSize = pageSize,
            Total = userCount
        };
    }

    public static User MapToUser(this UpdateUserRequest request, int id)
    {
        var age = CalculateAge(request.DateOfBirth);

        var personalData = new PersonalData
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = request.DateOfBirth
        };

        var email = new EmailAddress 
        { 
            Email = request.Email 
        };

        return new User
        {
            Id = id,
            PersonalData = personalData,
            EmailAddress = email
        };
    }

    public static GetAllUsersOptions MapToOptions(this GetAllUsersRequest request)
    {
        return new GetAllUsersOptions
        {
            Date = request.Date,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    private static int CalculateAge(DateTime dateOfBirth)
    {
        int age = _dateTime.UtcNow.Year - dateOfBirth.Year;

        if (_dateTime.UtcNow < dateOfBirth.AddYears(age))
        {
            age--;
        }

        return age;
    }
}
