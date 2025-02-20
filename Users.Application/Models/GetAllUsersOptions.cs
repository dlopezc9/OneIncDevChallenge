namespace Users.Application.Models;

public class GetAllUsersOptions
{
    public DateTime? Date { get; init; }
    public int Page { get; init; }

    public int PageSize { get; init; }
}
