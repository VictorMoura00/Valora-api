using Valora.Api.Extensions;
using Valora.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuração de Serviços (DI Container)
builder.Services.AddControllers();

builder.AddSerilogConfiguration();
builder.Services.AddDocumentation();                            // OpenAPI/Scalar
builder.Services.AddMongoDb(builder.Configuration);             // Conexão e Convenções
builder.Services.AddGlobalErrorHandler();                       // Tratamento de Erros
builder.Services.AddHealthMonitoring();                         // Health Checks
builder.Services.AddProjectDependencies(builder.Configuration);                      // Mapeiamento automatico de implementação de interface
builder.Services.AddApplication();                              // MediatR, Behaviors e Validators
builder.Services.AddCorsPolicy(builder.Configuration);          // Cors

var app = builder.Build();

// 2. Configuração do Pipeline HTTP
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