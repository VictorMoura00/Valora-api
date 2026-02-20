using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using Valora.Infra.Context;
using Valora.Infra.Repositories;

namespace Valora.UnitTests.Common.Stub.RepositoryStubs;

public class RepositoryStub : BaseRepository<EntityStub>
{
    public RepositoryStub(IMongoDatabase database, MongoContext context)
        : base(database, context, "test_collection")
    {
    }
}
