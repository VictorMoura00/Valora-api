using Serilog;

namespace Valora.Api.Extensions;

/// <summary>
/// Configuração do Serilog para logs no Console (Dev) e Application Insights (Prod).
/// </summary>
public static class SerilogExtensions
{
    public static void AddSerilogConfiguration(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();

        var loggerConfig = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration) 
            .Enrich.FromLogContext();

        if (!builder.Environment.IsDevelopment())
        {
            var appInsightsConnection = builder.Configuration["ApplicationInsights:ConnectionString"];
            
            if (!string.IsNullOrEmpty(appInsightsConnection))
            {
                loggerConfig.WriteTo.ApplicationInsights(
                    appInsightsConnection, 
                    TelemetryConverter.Traces); // Converte os logs do Serilog para Traces no Azure
            }
        }

        var logger = loggerConfig.CreateLogger();

        builder.Logging.AddSerilog(logger);
        builder.Host.UseSerilog(logger);
    }
}