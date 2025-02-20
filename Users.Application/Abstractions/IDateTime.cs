namespace Users.Application.Abstractions;

public interface IDateTime
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
}
