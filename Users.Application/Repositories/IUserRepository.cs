using Users.Application.Models;

namespace Users.Application.Repositories;

public interface IUserRepository
{
    Task<bool> CreateAsync(User user, CancellationToken token = default);
    Task<User?> GetByIdAsync(int id, CancellationToken token = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken token = default);
    Task<IReadOnlyCollection<User>> GetAllAsync(GetAllUsersOptions options, CancellationToken token = default);
    Task<bool> UpdateAsync(User user, CancellationToken token = default);
    Task<bool> DeleteByIdAsync(int id, CancellationToken token = default);
    Task<int> GetCountAsync(DateTime? date, CancellationToken token = default);
}
