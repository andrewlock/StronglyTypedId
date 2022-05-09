using System;
using Mongo2Go;
using MongoDB.Driver;
using Xunit;

namespace StronglyTypedIds.IntegrationTests.Fixtures;

public class MongoDbFixture : IDisposable
{
    private readonly MongoDbRunner _runner;

    public MongoDbFixture()
    {
        _runner = MongoDbRunner.Start();
        var client = new MongoClient(_runner.ConnectionString);
        Database = client.GetDatabase("IntegrationTest");
    }

    public IMongoDatabase Database { get; }

    public void Dispose()
    {
        _runner?.Dispose();
    }
}

[CollectionDefinition(Name)]
public class MongoDbCollection : ICollectionFixture<MongoDbFixture>
{
    public const string Name = "MongoDB collection";
}