namespace Valora.Api.Extensions;

public static class CorsExtensions
{
    private const string PolicyName = "AllowAll";

    public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, builder =>
            {
                // Em DESENVOLVIMENTO: Permite tudo (mais fácil)
                builder
                    .AllowAnyOrigin()  // Aceita requisição de qualquer lugar
                    .AllowAnyMethod()  // Aceita GET, POST, PUT, DELETE, etc.
                    .AllowAnyHeader(); // Aceita qualquer cabeçalho customizado

                /* * PARA PRODUÇÃO (Exemplo de como restringir):
                 * * builder
                 * .WithOrigins("https://meusite.com", "https://admin.meusite.com")
                 * .AllowAnyMethod()
                 * .AllowAnyHeader();
                 */
            });
        });

        return services;
    }

    public static WebApplication UseCorsPolicy(this WebApplication app)
    {
        app.UseCors(PolicyName);
        return app;
    }
}