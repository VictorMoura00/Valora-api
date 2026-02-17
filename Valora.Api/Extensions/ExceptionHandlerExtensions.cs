using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Valora.Api.Extensions;

public static class ExceptionHandlerExtensions
{
    public static IServiceCollection AddGlobalErrorHandler(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        return services;
    }
    
    public static WebApplication UseGlobalErrorHandler(this WebApplication app)
    {
        app.UseExceptionHandler(); // Usa o handler registrado acima
        return app;
    }
}

// Classe interna para lógica de tratamento
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Erro não tratado: {Message}", exception.Message);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Erro interno no servidor",
            Detail = "Ocorreu um erro inesperado. Tente novamente mais tarde."
            // Em DEV, você poderia adicionar exception.StackTrace aqui
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}