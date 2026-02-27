using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Valora.Api.Extensions;
using Valora.Api.Service;
using Valora.Application.Common.Interfaces;
using Valora.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);


builder.AddAzureKeyVaultSetup(); // Configuração de Cofre de Senhas (Azure Key Vault)
builder.Host.AddWolverineSetup(builder.Configuration); // Configuração do Host / Mensageria (Wolverine)

// Configuração de Serviços (DI Container)
builder.Services.AddControllers();

builder.AddSerilogConfiguration();
builder.Services.AddDocumentation();                            // OpenAPI/Scalar
builder.Services.AddMongoDb(builder.Configuration);             // Conexão e Convenções
builder.Services.AddGlobalErrorHandler();                       // Tratamento de Erros
builder.Services.AddHealthMonitoring();                         // Health Checks
builder.Services.AddProjectDependencies(builder.Configuration); // Mapeiamento automatico de implementação de interface
builder.Services.AddApplication();
builder.Services.AddCorsPolicy(builder.Configuration);          // Cors
builder.Services.AddHttpContextAccessor(); // Libera o acesso ao contexto HTTP atual para as classes injetadas

var app = builder.Build();

// Configuração do Pipeline HTTP
app.UseGlobalErrorHandler(); 
app.UseDocumentation();      // Swagger/Scalar (só em dev)
app.UseHealthMonitoring();   // Endpoint /health

//app.UseHttpsRedirection();
app.UseCorsPolicy();
//app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseStartupLog();

app.Run();
