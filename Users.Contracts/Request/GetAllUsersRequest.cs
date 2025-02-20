namespace Users.Contracts.Request;

public class GetAllUsersRequest : PagedRequest
{
    public DateTime? Date {  get; init; }

}
