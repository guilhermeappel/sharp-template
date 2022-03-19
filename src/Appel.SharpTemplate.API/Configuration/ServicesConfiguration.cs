
using Appel.SharpTemplate.API.Application.Interfaces;
using Appel.SharpTemplate.API.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Appel.SharpTemplate.API.Configuration;

public static class ServicesConfiguration
{
    public static IServiceCollection AddServicesConfiguration(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}
