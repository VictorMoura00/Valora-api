using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Wolverine;
using Wolverine.AzureServiceBus;
using Wolverine.FluentValidation;
using Valora.Application.Extensions; // Necessário para referenciar um tipo da camada Application

namespace Valora.Api.Extensions;

/// <summary>
/// Extensão para configuração do Wolverine e barramento de mensagens.
/// Autor: Victor Moura
/// </summary>
public static class WolverineExtensions
{
    public static IHostBuilder AddWolverineSetup(this IHostBuilder hostBuilder, IConfiguration configuration)
    {
        return hostBuilder.UseWolverine(opts =>
        {
            // 1. Ensina o Wolverine a escanear a camada Application para encontrar os Handlers
            opts.Discovery.IncludeAssembly(typeof(ApplicationExtensions).Assembly);

            // 2. Intercepta os comandos e roda os validadores do FluentValidation automaticamente
            opts.UseFluentValidation();

            // 3. Integração com Azure Service Bus
            var asbConnectionString = configuration.GetConnectionString("AzureServiceBus");

            if (!string.IsNullOrEmpty(asbConnectionString))
            {
                opts.UseAzureServiceBus(asbConnectionString)
                    .AutoProvision(); // Cria filas/tópicos automaticamente se não existirem na Azure
            }
        });
    }
}