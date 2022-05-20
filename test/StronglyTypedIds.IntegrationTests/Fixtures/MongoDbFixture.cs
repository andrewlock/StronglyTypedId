using System;
using System.Runtime.InteropServices;
using Mongo2Go;
using MongoDB.Driver;
using Xunit;

namespace StronglyTypedIds.IntegrationTests.Fixtures;

public class MongoDbFixture : IDisposable
{
    private readonly MongoDbRunner _runner;

    public MongoDbFixture()
    {
        _runner = MongoDbRunner.Start(binariesSearchPatternOverride: GetBinariesSearchPattern());
        var client = new MongoClient(_runner.ConnectionString);
        Database = client.GetDatabase("IntegrationTest");
    }

    public IMongoDatabase Database { get; }

    public void Dispose()
    {
        _runner?.Dispose();
    }

    // Overrode default binaries search pattern because of problem on some Linux OS versions
    private static string GetBinariesSearchPattern()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "tools/mongodb-linux*/bin" : default;
    }
}

[CollectionDefinition(Name)]
public class MongoDbCollection : ICollectionFixture<MongoDbFixture>
{
    public const string Name = "MongoDB collection";
}