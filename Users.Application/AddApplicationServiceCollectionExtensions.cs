using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Users.Application.Abstractions;
using Users.Application.Repositories;
using Users.Application.Services;

namespace Users.Application;

public static class AddApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IDateTime, SystemDateTime>();
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<IUserService, UserService>();
        services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Singleton);
        return services;
    }
}
