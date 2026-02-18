using Scalar.AspNetCore;

namespace Valora.Api.Extensions;

public static class OpenApiExtensions
{
    public static IServiceCollection AddDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info.Title = "Valora API";
                document.Info.Version = "v1";
                document.Info.Description = "API do Ecossistema Valora - Avaliação Universal de Itens";
                return Task.CompletedTask;
            });
        });

        return services;
    }

    public static WebApplication UseDocumentation(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            // Acessível em: http://localhost:8080/scalar/v1
            app.MapScalarApiReference(options =>
            {
                options
                    .WithTitle("Valora API Docs")
                    .WithTheme(ScalarTheme.Laserwave)
                    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
            });
        }

        return app;
    }
}