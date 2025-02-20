using FluentValidation;
using Users.Application.Models;
using Users.Application.Repositories;

namespace Users.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<User> _userValidator;
    private readonly IValidator<GetAllUsersOptions> _userOptionsValidator;

    public UserService(IUserRepository userRepository, 
                       IValidator<User> userValidator,
                       IValidator<GetAllUsersOptions> userOptionsValidator)
    {
        _userRepository = userRepository;
        _userValidator = userValidator;
        _userOptionsValidator = userOptionsValidator;
    }

    public async Task<bool> CreateAsync(User user, CancellationToken token = default)
    {
        await _userValidator.ValidateAndThrowAsync(user, token);
        return await _userRepository.CreateAsync(user, token);
    }

    public Task<bool> DeleteByIdAsync(int id, CancellationToken token = default)
    {
        return _userRepository.DeleteByIdAsync(id, token);
    }

    public async Task<IReadOnlyCollection<User>> GetAllAsync(GetAllUsersOptions options, CancellationToken token = default)
    {
        await _userOptionsValidator.ValidateAndThrowAsync(options, token);
        return await _userRepository.GetAllAsync(options, token);
    }

    public Task<User?> GetByIdAsync(int id, CancellationToken token = default)
    {
        return _userRepository.GetByIdAsync(id, token);
    }

    public Task<int> GetCountAsync(DateTime? date, CancellationToken token = default)
    {
        return _userRepository.GetCountAsync(date, token);
    }

    public async Task<User?> UpdateAsync(User user, CancellationToken token = default)
    {
        await _userValidator.ValidateAndThrowAsync(user, token);

        var userExist = await _userRepository.GetByIdAsync(user.Id, token);
        if (userExist is null)
        {
            return null;
        }
        await _userRepository.UpdateAsync(user, token);
        return user;
    }
}
