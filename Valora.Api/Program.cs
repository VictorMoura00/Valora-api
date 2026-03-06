using Valora.Api.Extensions;
using Valora.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddAzureKeyVaultSetup();
builder.Host.AddWolverineSetup(builder.Configuration);

// Configuração de Serviços (DI Container)
builder.Services.AddControllers();

builder.AddSerilogConfiguration();
builder.Services.AddDocumentation();
builder.Services.AddMongoDb(builder.Configuration);
builder.Services.AddGlobalErrorHandler();
builder.Services.AddHealthMonitoring();
builder.Services.AddProjectDependencies(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddCorsPolicy(builder.Configuration);
builder.Services.AddApiAuthentication(builder.Configuration);

var app = builder.Build();

// Configuração do Pipeline HTTP
app.UseGlobalErrorHandler();
app.UseDocumentation();
app.UseHealthMonitoring();

//app.UseHttpsRedirection();
app.UseCorsPolicy();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseStartupLog();

app.Run();
