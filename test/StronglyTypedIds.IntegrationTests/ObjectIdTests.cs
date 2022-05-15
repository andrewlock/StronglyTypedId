using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using StronglyTypedIds.IntegrationTests.Fixtures;
using StronglyTypedIds.IntegrationTests.Types;
using Xunit;
using NewtonsoftJsonSerializer = Newtonsoft.Json.JsonConvert;
using SystemTextJsonSerializer = System.Text.Json.JsonSerializer;

namespace StronglyTypedIds.IntegrationTests;

[Collection(MongoDbCollection.Name)]
public class ObjectIdTests
{
    private readonly MongoDbFixture _mongoDbFixture;

    public ObjectIdTests(MongoDbFixture mongoDbFixture)
    {
        _mongoDbFixture = mongoDbFixture;
    }
    
    [Fact]
    public void SameValuesAreEqual()
    {
        var id = ObjectId.GenerateNewId();
        var foo1 = new ObjectIdId1(id);
        var foo2 = new ObjectIdId1(id);

        Assert.Equal(foo1, foo2);
    }

    [Fact]
    public void EmptyValueIsEmpty()
    {
        Assert.Equal(ObjectIdId1.Empty.Value, ObjectId.Empty);
    }


    [Fact]
    public void DifferentValuesAreUnequal()
    {
        var foo1 = ObjectIdId1.New();
        var foo2 = ObjectIdId1.New();

        Assert.NotEqual(foo1, foo2);
    }

    [Fact]
    public void OverloadsWorkCorrectly()
    {
        var id = ObjectId.GenerateNewId();
        var same1 = new ObjectIdId1(id);
        var same2 = new ObjectIdId1(id);
        var different = ObjectIdId1.New();

        Assert.True(same1 == same2);
        Assert.False(same1 == different);
        Assert.False(same1 != same2);
        Assert.True(same1 != different);
    }

    [Fact]
    public void DifferentTypesAreUnequal()
    {
        var bar = ObjectIdId2.New();
        var foo = ObjectIdId1.New();

        //Assert.NotEqual(bar, foo); // does not compile
        Assert.NotEqual((object)bar, (object)foo);
    }

    [Fact]
    public void CantCreateEmptyGeneratedId1()
    {
        var foo = new ObjectIdId1();
        var bar = new ObjectIdId2();

        //Assert.NotEqual(bar, foo); // does not compile
        Assert.NotEqual((object)bar, (object)foo);
    }

    [Fact]
    public void CanSerializeToObjectId_WithTypeConverter()
    {
        var foo = NewtonsoftJsonObjectIdId.New();

        var serializedFoo = NewtonsoftJsonSerializer.SerializeObject(foo);
        var serializedObjectId = NewtonsoftJsonSerializer.SerializeObject(foo.Value.ToString());

        Assert.Equal(serializedFoo, serializedObjectId);
    }
    
    [Fact]
    public void CanSerializeToNullable_WithNewtonsoftJsonProvider()
    {
        var entity = new EntityWithNullableId { Id = null };

        var json = NewtonsoftJsonSerializer.SerializeObject(entity);
        var deserialize = NewtonsoftJsonSerializer.DeserializeObject<EntityWithNullableId>(json);

        Assert.NotNull(deserialize);
        Assert.Equal(deserialize.Id, NewtonsoftJsonObjectIdId.Empty);
    }

    [Fact]
    public void CanSerializeToObjectId_WithSystemTextJsonProvider()
    {
        var foo = SystemTextJsonObjectIdId.New();

        var serializedFoo = SystemTextJsonSerializer.Serialize(foo);
        var serializedObjectId = SystemTextJsonSerializer.Serialize(foo.Value.ToString());

        Assert.Equal(serializedFoo, serializedObjectId);
    }

    [Fact]
    public void CanDeserializeFromObjectId_WithNewtonsoftJsonProvider()
    {
        var value = ObjectId.GenerateNewId();
        var foo = new NewtonsoftJsonObjectIdId(value);
        var serializedObjectId = NewtonsoftJsonSerializer.SerializeObject(value);

        var deserializedFoo = NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonObjectIdId>(serializedObjectId);

        Assert.Equal(foo, deserializedFoo);
    }

    [Fact]
    public void CanDeserializeFromObjectId_WithSystemTextJsonProvider()
    {
        var value = ObjectId.GenerateNewId();
        var foo = new SystemTextJsonObjectIdId(value);
        var serializedObjectId = SystemTextJsonSerializer.Serialize(value.ToString());

        var deserializedFoo = SystemTextJsonSerializer.Deserialize<SystemTextJsonObjectIdId>(serializedObjectId);

        Assert.Equal(foo, deserializedFoo);
    }

    [Fact]
    public void CanSerializeToObjectId_WithBothJsonConverters()
    {
        var foo = BothJsonObjectIdId.New();

        var serializedFoo1 = NewtonsoftJsonSerializer.SerializeObject(foo);
        var serializedObjectId1 = NewtonsoftJsonSerializer.SerializeObject(foo.Value.ToString());

        var serializedFoo2 = SystemTextJsonSerializer.Serialize(foo);
        var serializedObjectId2 = SystemTextJsonSerializer.Serialize(foo.Value.ToString());

        Assert.Equal(serializedFoo1, serializedObjectId1);
        Assert.Equal(serializedFoo2, serializedObjectId2);
    }

    [Fact]
    public void WhenNoJsonConverter_SystemTextJsonSerializesWithValueProperty()
    {
        var foo = NoJsonObjectIdId.New();

        var serialized = SystemTextJsonSerializer.Serialize(foo);

        var expected = "{\"Value\":" + SystemTextJsonSerializer.Serialize(foo.Value) + "}";

        Assert.Equal(expected, serialized);
    }

    [Fact]
    public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
    {
        var foo = NoJsonObjectIdId.New();

        var serialized = NewtonsoftJsonSerializer.SerializeObject(foo);

        var expected =  $"\"{foo.Value.ToString()}\"";

        Assert.Equal(expected, serialized);
    }

    [Fact]
    public void WhenNoTypeConverter_SerializesWithValueProperty()
    {
        var foo = NoConverterObjectIdId.New();

        var newtonsoft = SystemTextJsonSerializer.Serialize(foo);
        var systemText = SystemTextJsonSerializer.Serialize(foo);

        var expected = "{\"Value\":" + SystemTextJsonSerializer.Serialize(foo.Value) + "}";

        Assert.Equal(expected, newtonsoft);
        Assert.Equal(expected, systemText);
    }

    [Fact]
    public void WhenEfCoreValueConverterUsesValueConverter()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(connection)
            .Options;

        using (var context = new TestDbContext(options))
        {
            context.Database.EnsureCreated();
            context.Entities.Add(
                new TestEntity { Id = EfCoreObjectIdId.New() });
            context.SaveChanges();
        }
        using (var context = new TestDbContext(options))
        {
            var all = context.Entities.ToList();
            Assert.Single(all);
        }
    }

    [Fact]
    public async Task WhenDapperValueConverterUsesValueConverter()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var results = await connection.QueryAsync<DapperObjectIdId>("SELECT '62758c6ee39f6ef4751bb831'");

        var value = Assert.Single(results);
        Assert.Equal(new DapperObjectIdId(new ObjectId("62758c6ee39f6ef4751bb831")), value);
    }
    
    [Fact]
    public async Task WhenMongoSerializerUsesSerializer()
    {
        var collection = _mongoDbFixture.Database.GetCollection<TestDocument>("ObjectIdIdTestCollection");

        var objectIdValue = ObjectId.GenerateNewId();
        var original = new TestDocument { Id = new MongoObjectIdId(objectIdValue) };
        await collection.InsertOneAsync(original);
        var retrieved = await collection.Find(x => x.Id == new MongoObjectIdId(objectIdValue)).FirstAsync();
        
        Assert.Equal(new MongoObjectIdId(objectIdValue), retrieved.Id);
    }

    [Theory]
    [InlineData("62758cacb6f910fe91bd697b")]
    public void TypeConverter_CanConvertToAndFrom(string value)
    {
        var converter = TypeDescriptor.GetConverter(typeof(NoJsonObjectIdId));
        var id = converter.ConvertFrom(value);
        Assert.IsType<NoJsonObjectIdId>(id);
        Assert.Equal(new NoJsonObjectIdId(new ObjectId(value)), id);

        var reconverted = converter.ConvertTo(id, value.GetType());
        Assert.Equal(value, reconverted);
    }

    [Fact]
    public void CanCompareDefaults()
    {
        ComparableObjectIdId original = default;
        var other = ComparableObjectIdId.Empty;

        var compare1 = original.CompareTo(other);
        var compare2 = other.CompareTo(original);
        Assert.Equal(compare1, -compare2);
    }

    [Fact]
    public void CanEquateDefaults()
    {
        EquatableObjectIdId original = default;
        var other = EquatableObjectIdId.Empty;

        var equals1 = (original as IEquatable<EquatableObjectIdId>).Equals(other);
        var equals2 = (other as IEquatable<EquatableObjectIdId>).Equals(original);

        Assert.Equal(equals1, equals2);
    }

    [Fact]
    public void ImplementsInterfaces()
    {
        Assert.IsAssignableFrom<IEquatable<BothObjectIdId>>(BothObjectIdId.Empty);
        Assert.IsAssignableFrom<IComparable<BothObjectIdId>>(BothObjectIdId.Empty);

        Assert.IsAssignableFrom<IEquatable<EquatableObjectIdId>>(EquatableObjectIdId.Empty);
        Assert.IsAssignableFrom<IComparable<ComparableObjectIdId>>(ComparableObjectIdId.Empty);

#pragma warning disable 184
        Assert.False(ObjectIdId1.Empty is IComparable<ObjectIdId1>);
        Assert.False(ObjectIdId1.Empty is IEquatable<ObjectIdId1>);
#pragma warning restore 184
    }


#if NET6_0_OR_GREATER
    [Fact]
    public void WhenConventionBasedEfCoreValueConverterUsesValueConverter()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<ConventionsDbContext>()
            .UseSqlite(connection)
            .Options;

        using (var context = new ConventionsDbContext(options))
        {
            context.Database.EnsureCreated();
            context.Entities.Add(
                new TestEntity { Id = EfCoreObjectIdId.New() });
            context.SaveChanges();
        }
        using (var context = new ConventionsDbContext(options))
        {
            var all = context.Entities.ToList();
            Assert.Single(all);
        }
    }

    public class ConventionsDbContext : DbContext
    {
        public DbSet<TestEntity> Entities { get; set; }

        public ConventionsDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder
                .Properties<EfCoreObjectIdId>()
                .HaveConversion<EfCoreObjectIdId.EfCoreValueConverter>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<TestEntity>(builder =>
                {
                    builder
                        .Property(x => x.Id)
                        .ValueGeneratedNever();
                });
        }
    }
#endif

    public class TestDbContext : DbContext
    {
        public DbSet<TestEntity> Entities { get; set; }

        public TestDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<TestEntity>(builder =>
                {
                    builder
                        .Property(x => x.Id)
                        .HasConversion(new EfCoreObjectIdId.EfCoreValueConverter())
                        .ValueGeneratedNever();
                });
        }
    }

    public class TestEntity
    {
        public EfCoreObjectIdId Id { get; set; }
    }
    
    public class EntityWithNullableId
    {
        public NewtonsoftJsonObjectIdId? Id { get; set; }
    }

    public class TestDocument
    {
        public MongoObjectIdId Id { get; set; }
    }
}