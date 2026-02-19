using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Wolverine;
using Wolverine.AzureServiceBus;
using Wolverine.FluentValidation;

namespace Valora.Api.Extensions;

/// <summary>
/// Extensão para configuração do Wolverine e barramento de mensagens.
/// </summary>
public static class WolverineExtensions
{
    public static IHostBuilder AddWolverineSetup(this IHostBuilder hostBuilder, IConfiguration configuration)
    {
        return hostBuilder.UseWolverine(opts =>
        {
            opts.UseFluentValidation();

            var asbConnectionString = configuration.GetConnectionString("AzureServiceBus");

            if (!string.IsNullOrEmpty(asbConnectionString))
            {
                opts.UseAzureServiceBus(asbConnectionString)
                    .AutoProvision(); // Cria filas/tópicos automaticamente se não existirem na Azure
            }
        });
    }
}