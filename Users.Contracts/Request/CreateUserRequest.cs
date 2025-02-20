namespace Users.Contracts.Request;

public class CreateUserRequest
{
    public required string FirstName { get; init; }
    public string? LastName { get; init;}
    public required string Email { get; init; }
    public required DateTime DateOfBirth { get; init; }
    public required string PhoneNumber { get; init; }
}
