using Users.Application.Models;

namespace Users.Application.Services;

public interface IUserService
{
    Task<bool> CreateAsync(User user, CancellationToken token = default);
    Task<User?> GetByIdAsync(int id, CancellationToken token = default);
    Task<IReadOnlyCollection<User>> GetAllAsync(GetAllUsersOptions options, CancellationToken token = default);
    Task<User?> UpdateAsync(User user, CancellationToken token = default);
    Task<bool> DeleteByIdAsync(int id, CancellationToken token = default);
    Task<int> GetCountAsync(DateTime? date, CancellationToken token = default);
}
