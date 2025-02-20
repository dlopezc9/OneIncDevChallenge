namespace Users.Application.Models;

public class User
{
    public required int Id { get; set; }
    public required PersonalData PersonalData {  get; set; }
    public required EmailAddress EmailAddress { get; set; }
}
