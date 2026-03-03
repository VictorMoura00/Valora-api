using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using Valora.Infra.Context;
using Valora.Infra.Repositories;
using Valora.UnitTests.Common.Stub.EntityStubs;

namespace Valora.UnitTests.Common.Stub.RepositoryStubs;

public class RepositoryStub(IMongoDatabase database, MongoContext context)
    : BaseRepository<EntityStub>(database, context, "test_collection");
