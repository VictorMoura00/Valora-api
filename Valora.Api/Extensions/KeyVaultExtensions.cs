using System;
using Azure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Valora.Api.Extensions;

/// <summary>
/// Extensão para configuração do Azure Key Vault.
/// Autor: Victor Moura
/// </summary>
public static class KeyVaultExtensions
{
    public static WebApplicationBuilder AddAzureKeyVaultSetup(this WebApplicationBuilder builder)
    {
        var keyVaultName = builder.Configuration["KeyVaultName"];

        if (!string.IsNullOrEmpty(keyVaultName))
        {
            var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");

            // DefaultAzureCredential tentará autenticar usando:
            // 1. Variáveis de ambiente
            // 2. Identidade Gerenciada (quando rodando na Azure)
            // 3. Credenciais do Visual Studio / CLI (localmente)
            builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());
        }

        return builder;
    }
}