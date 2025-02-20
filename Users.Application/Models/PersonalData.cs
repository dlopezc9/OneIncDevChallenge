namespace Users.Application.Models;

public class PersonalData
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string PhoneNumber { get; set; }
    public required DateTime DateOfBirth { get; set; }
}
