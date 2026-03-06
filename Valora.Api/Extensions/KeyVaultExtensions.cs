using Azure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting; // Necessário para IsDevelopment()

namespace Valora.Api.Extensions;

public static class KeyVaultExtensions
{
    public static WebApplicationBuilder AddAzureKeyVaultSetup(this WebApplicationBuilder builder)
    {
        // Só tenta conectar ao Azure Key Vault se NÃO for ambiente de desenvolvimento
        if (!builder.Environment.IsDevelopment())
        {
            var keyVaultName = builder.Configuration["KeyVaultName"];

            if (!string.IsNullOrEmpty(keyVaultName))
            {
                builder.Configuration.AddAzureKeyVault(
                    new Uri($"https://{keyVaultName}.vault.azure.net/"),
                    new DefaultAzureCredential());
            }
        }

        return builder;
    }
}
