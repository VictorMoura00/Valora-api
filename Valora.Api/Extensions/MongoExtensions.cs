using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Valora.Infra.Options;

namespace Valora.Api.Extensions;

public static class MongoExtensions
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        #pragma warning disable CS0618 
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        #pragma warning restore CS0618

        var pack = new ConventionPack
        {
            new CamelCaseElementNameConvention(),
            new IgnoreExtraElementsConvention(true),
            new EnumRepresentationConvention(BsonType.String)
        };
        ConventionRegistry.Register("ValoraConventions", pack, t => true);

        // 3. Configura o Options Pattern
        services.Configure<MongoSettings>(
            configuration.GetSection(MongoSettings.SectionName));

        // 4. Injeta o Cliente (Singleton)
        services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = configuration.GetSection(MongoSettings.SectionName).Get<MongoSettings>();
            return new MongoClient(settings!.ConnectionString);
        });

        // 5. Injeta o Banco de Dados (Scoped)
        services.AddScoped<IMongoDatabase>(sp =>
        {
            var settings = configuration.GetSection(MongoSettings.SectionName).Get<MongoSettings>();
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(settings!.DatabaseName);
        });

        return services;
    }
}