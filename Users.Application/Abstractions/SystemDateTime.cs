namespace Users.Application.Abstractions;

public class SystemDateTime : IDateTime
{
    public DateTime Now => TimeProvider.System.GetLocalNow().DateTime;
    public DateTime UtcNow => TimeProvider.System.GetUtcNow().DateTime;
}
