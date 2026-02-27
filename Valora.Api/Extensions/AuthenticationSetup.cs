using Microsoft.AspNetCore.Authentication.JwtBearer;
using Valora.Api.Service;
using Valora.Application.Common.Interfaces;

namespace Valora.Api.Extensions;

public static class AuthenticationSetup
{
    public static IServiceCollection AddApiAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["Auth0:Domain"];
                options.Audience = configuration["Auth0:Audience"];
            });

        services.AddAuthorization();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
