using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Valora.Application.Extensions;

/// <summary>
/// Configuração da camada de aplicação.
/// </summary>
public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(ApplicationExtensions).Assembly;

        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}