using System.Reflection;
using Valora.Domain.Common.Interfaces;
using Valora.Infra.Context;
using Valora.Infra.Options;

namespace Valora.Api.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddProjectDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Vincula o JSON à classe de Settings (Necessário para o MongoContext)
        services.Configure<MongoSettings>(configuration.GetSection(MongoSettings.SectionName));

        // 2. Registra o MongoContext e o UnitOfWork de forma Scoped
        services.AddScoped<MongoContext>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<MongoContext>());
        var applicationAssembly = Assembly.Load("Valora.Application");
        var infraAssembly = Assembly.Load("Valora.Infra");

        services.Scan(scan => scan
            // 1. Onde procurar? (Application e Infra)
            .FromAssemblies(applicationAssembly, infraAssembly)

            // 2. O que registrar? (Regra dos Services)
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
            .AsImplementedInterfaces() // Ex: ItemService conta como IItemService
            .WithScopedLifetime()

            // 3. O que registrar? (Regra dos Repositories)
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
            .AsImplementedInterfaces() // Ex: ItemRepository conta como IItemRepository
            .WithScopedLifetime()
        );

        return services;
    }
}