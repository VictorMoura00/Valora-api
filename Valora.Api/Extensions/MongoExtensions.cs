using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Valora.Infra.Options; // Certifique-se de ter essa referência

namespace Valora.Api.Extensions;

public static class MongoExtensions
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Configura as Convenções do BSON (Globalmente)
        // Isso garante que "NomeCompleto" no C# vire "nomeCompleto" no Mongo
        // e que Enums sejam salvos como String (mais legível) em vez de Int.
        var pack = new ConventionPack
        {
            new CamelCaseElementNameConvention(),
            new IgnoreExtraElementsConvention(true),
            new EnumRepresentationConvention(BsonType.String)
        };
        ConventionRegistry.Register("ValoraConventions", pack, t => true);

        // 2. Configura o Options Pattern (Bind do appsettings.json)
        services.Configure<MongoSettings>(
            configuration.GetSection(MongoSettings.SectionName));

        // 3. Injeta o Cliente (Singleton - recomendado pela doc do Mongo)
        services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = configuration.GetSection(MongoSettings.SectionName).Get<MongoSettings>();
            return new MongoClient(settings!.ConnectionString);
        });

        // 4. Injeta o Banco de Dados (Scoped - para uso nos repositórios)
        services.AddScoped<IMongoDatabase>(sp =>
        {
            var settings = configuration.GetSection(MongoSettings.SectionName).Get<MongoSettings>();
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(settings!.DatabaseName);
        });

        return services;
    }
}