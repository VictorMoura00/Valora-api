using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Valora.Domain.Entities;
using Valora.Domain.Repositories;
using Valora.Infra.Context;

namespace Valora.Infra.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(IMongoDatabase database, MongoContext context)
        : base(database, context, "users")
    {
    }

    public async Task<bool> IsNicknameTakenAsync(
        string nickname,
        Guid? excludeUserId = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(nickname))
            return false;

        var filter = ActiveOnlyFilter;

        var nicknameFilter = Builders<User>.Filter.Regex(
            x => x.Nickname,
            new MongoDB.Bson.BsonRegularExpression($"^{nickname.Trim()}$", "i"));

        filter = Builders<User>.Filter.And(filter, nicknameFilter);

        if (excludeUserId.HasValue)
        {
            var excludeSelfFilter = Builders<User>.Filter.Ne(x => x.Id, excludeUserId.Value);
            filter = Builders<User>.Filter.And(filter, excludeSelfFilter);
        }

        return await _collection.Find(filter).AnyAsync(cancellationToken);
    }
}

