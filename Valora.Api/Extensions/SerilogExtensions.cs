using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Valora.Api.Extensions;

public static class SerilogExtensions
{
    public static void AddSerilogConfiguration(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();

        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration) 
            .Enrich.FromLogContext()
            .CreateLogger();

        builder.Logging.AddSerilog(logger);
        
        builder.Host.UseSerilog(logger);
    }
}