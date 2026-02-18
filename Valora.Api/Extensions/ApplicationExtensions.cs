using System.Reflection;
using FluentValidation;
using Valora.Application.Behaviors;

namespace Valora.Api.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Pega o Assembly atual (Valora.Application)
        var assembly = Assembly.GetExecutingAssembly();

        // 1. Registra o MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            
            // 2. Adiciona o Behavior de Validação no Pipeline
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // 3. Registra todos os Validadores automaticamente
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}