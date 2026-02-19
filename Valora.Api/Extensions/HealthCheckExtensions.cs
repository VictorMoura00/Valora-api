using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver; // Necessário para IMongoClient

namespace Valora.Api.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddHealthMonitoring(this IServiceCollection services)
    {
        services.AddHealthChecks()
            // CORREÇÃO: Em vez de passar a string, pedimos o cliente injetado (Singleton)
            // Isso casa com a assinatura: AddMongoDb(Func<IServiceProvider, IMongoClient> factory, ...)
            .AddMongoDb(
                sp => sp.GetRequiredService<IMongoClient>(),
                name: "mongodb",
                timeout: TimeSpan.FromSeconds(3)
            );

        return services;
    }

    public static WebApplication UseHealthMonitoring(this WebApplication app)
    {
        app.MapHealthChecks("/health");
        return app;
    }
}