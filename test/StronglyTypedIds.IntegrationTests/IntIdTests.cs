using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using StronglyTypedIds.IntegrationTests.Types;
using Xunit;
using NewtonsoftJsonSerializer = Newtonsoft.Json.JsonConvert;
using SystemTextJsonSerializer = System.Text.Json.JsonSerializer;

namespace StronglyTypedIds.IntegrationTests;

public partial class IntIdTests
{
    [Fact]
    public void SameValuesAreEqual()
    {
        var id = 123;
        var foo1 = new IntId(id);
        var foo2 = new IntId(id);

        Assert.Equal(foo1, foo2);
    }

    [Fact]
    public void EmptyValueIsEmpty()
    {
        Assert.Equal(0, IntId.Empty.Value);
    }


    [Fact]
    public void DifferentValuesAreUnequal()
    {
        var foo1 = new IntId(1);
        var foo2 = new IntId(2);

        Assert.NotEqual(foo1, foo2);
    }

    [Fact]
    public void OverloadsWorkCorrectly()
    {
        var id = 12;
        var same1 = new IntId(id);
        var same2 = new IntId(id);
        var different = new IntId(3);

        Assert.True(same1 == same2);
        Assert.False(same1 == different);
        Assert.False(same1 != same2);
        Assert.True(same1 != different);
    }

    [Fact]
    public void DifferentTypesAreUnequal()
    {
        var bar = GuidId2.New();
        var foo = new IntId(23);

        //Assert.NotEqual(bar, foo); // does not compile
        Assert.NotEqual((object) bar, (object) foo);
    }

#if NET8_0_OR_GREATER
    [Fact]
    public void CanRoundTripUtf8()
    {
        var id = new IntId(123);

        var actual = new byte[16].AsSpan();
        Assert.True(id.TryFormat(actual, out var charsWritten));

        var success = IntId.TryParse(actual.Slice(0, charsWritten), provider: null, out var result);

        Assert.True(success);
        Assert.Equal(id, result);
    }
#endif

    [Fact]
    public void CanSerializeToNullableInt_WithNewtonsoftJsonProvider()
    {
        var entity = new EntityWithNullableId {Id = null};

        var json = NewtonsoftJsonSerializer.SerializeObject(entity);
        var deserialize = NewtonsoftJsonSerializer.DeserializeObject<EntityWithNullableId>(json);

        Assert.NotNull(deserialize);
        Assert.Null(deserialize.Id);
    }

#if NET6_0_OR_GREATER
    [Fact]
    public void CanDeserializeDictionaryKeys_WithSystemTextJsonProvider()
    {
        var value = new TypeWithDictionaryKeys()
        {
            Values = new()
        };
        var key = new IntId(123);
        value.Values.Add(key, "My Value");
        var opts = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        var serialized = SystemTextJsonSerializer.Serialize(value, opts);

        var expected = $$"""
                         {
                           "values": {
                             "123": "My Value"
                           }
                         }
                         """;
        Assert.Equal(serialized, expected);

        var deserialized = SystemTextJsonSerializer.Deserialize<TypeWithDictionaryKeys>(serialized, opts);

        Assert.NotNull(deserialized.Values);
        Assert.True(deserialized.Values.ContainsKey(key));
        Assert.Equal("My Value", deserialized.Values[key]);
    }
#endif

    [Fact]
    public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
    {
        var foo = new IntId(123);

        var serialized = NewtonsoftJsonSerializer.SerializeObject(foo);

        var expected = $"\"{foo.Value}\"";

        Assert.Equal(expected, serialized);
    }

    [Fact]
    public void CanCompareDefaults()
    {
        IntId original = default;
        var other = IntId.Empty;

        var compare1 = original.CompareTo(other);
        var compare2 = other.CompareTo(original);
        Assert.Equal(compare1, -compare2);
    }

    [Fact]
    public void CanEquateDefaults()
    {
        IntId original = default;
        var other = IntId.Empty;

        var equals1 = (original as IEquatable<IntId>).Equals(other);
        var equals2 = (other as IEquatable<IntId>).Equals(original);

        Assert.Equal(equals1, equals2);
    }

    [Fact]
    public void ImplementsInterfaces()
    {
        Assert.IsAssignableFrom<IEquatable<IntId>>(IntId.Empty);
        Assert.IsAssignableFrom<IComparable<IntId>>(IntId.Empty);
        Assert.IsAssignableFrom<IFormattable>(IntId.Empty);

#pragma warning disable CS0183
#pragma warning disable 184
        Assert.True(IntId.Empty is IComparable<IntId>);
        Assert.True(IntId.Empty is IEquatable<IntId>);
#pragma warning restore 184
#pragma warning restore CS0183

#if NET6_0_OR_GREATER
        Assert.IsAssignableFrom<ISpanFormattable>(IntId.Empty);
#endif
#if NET7_0_OR_GREATER
        // doesn't compile if doesn't implement it 
        ParseAs<IntId>("123");
        ParseSpan<IntId>("123".AsSpan());

        T ParseAs<T>(string s) where T : IParsable<T>
        {
            return T.Parse(s, null);
        }

        T ParseSpan<T>(ReadOnlySpan<char> s) where T : ISpanParsable<T>
        {
            return T.Parse(s, null);
        }
#endif
    }

#region ConvertersIntId
    [Fact]
    public void CanSerializeToInt_WithNewtonsoftJsonProvider()
    {
        var foo = new ConvertersIntId(123);

        var serializedFoo = NewtonsoftJsonSerializer.SerializeObject(foo);
        var serializedInt = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

        Assert.Equal(serializedFoo, serializedInt);
    }

    [Fact]
    public void CanSerializeToInt_WithSystemTextJsonProvider()
    {
        var foo = new ConvertersIntId(123);

        var serializedFoo = SystemTextJsonSerializer.Serialize(foo);
        var serializedInt = SystemTextJsonSerializer.Serialize(foo.Value);

        Assert.Equal(serializedFoo, serializedInt);
    }

    [Fact]
    public void CanDeserializeFromInt_WithNewtonsoftJsonProvider()
    {
        var value = 123;
        var foo = new ConvertersIntId(value);
        var serializedInt = NewtonsoftJsonSerializer.SerializeObject(value);

        var deserializedFoo = NewtonsoftJsonSerializer.DeserializeObject<ConvertersIntId>(serializedInt);

        Assert.Equal(foo, deserializedFoo);
    }

    [Fact]
    public void CanDeserializeFromInt_WithSystemTextJsonProvider()
    {
        var value = 123;
        var foo = new ConvertersIntId(value);
        var serializedInt = SystemTextJsonSerializer.Serialize(value);

        var deserializedFoo = SystemTextJsonSerializer.Deserialize<ConvertersIntId>(serializedInt);

        Assert.Equal(foo, deserializedFoo);
    }

    [Fact]
    public void WhenEfCoreValueConverterUsesValueConverter()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(connection)
            .Options;

        var original = new TestEntity {Id = new ConvertersIntId(123)};
        using (var context = new TestDbContext(options))
        {
            context.Database.EnsureCreated();
            context.Entities.Add(original);
            context.SaveChanges();
        }

        using (var context = new TestDbContext(options))
        {
            var all = context.Entities.ToList();
            var retrieved = Assert.Single(all);
            Assert.Equal(original.Id, retrieved.Id);
        }
    }

    [Fact]
    public Task WhenDapperValueConverterUsesValueConverter_Id()
        => WhenDapperValueConverterUsesValueConverter(g => new ConvertersIntId(g));

    [Fact]
    public Task WhenDapperValueConverterUsesValueConverter_Converter()
        => WhenDapperValueConverterUsesValueConverter(g => new IntId(g));
        
    private async Task WhenDapperValueConverterUsesValueConverter<T>(Func<int, T> newFunc)
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var results = await connection.QueryAsync<T>("SELECT 123");

        var value = Assert.Single(results);
        Assert.Equal(newFunc(123), value);
    }

    [Fact(Skip = "Requires localdb to be available")]
    public async Task WhenDapperValueConverterUsesValueConverterWithSqlServer()
    {
        using var connection = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30");
        await connection.OpenAsync();

        var results = await connection.QueryAsync<ConvertersIntId>("SELECT CAST (123 AS numeric(38,0))");

        var value = Assert.Single(results);
        Assert.Equal(new ConvertersIntId(123), value);
    }

    [Fact]
    public void WhenDapperValueConverterAndDecimalUsesValueConverter()
    {
        var handler = new ConvertersIntId.DapperTypeHandler();
        var value = handler.Parse((decimal) 123L);

        Assert.Equal(new ConvertersIntId(123), value);
    }
    
    [Theory]
    [InlineData(123)]
    [InlineData("123")]
    public void TypeConverter_CanConvertToAndFrom(object value)
    {
        var converter = TypeDescriptor.GetConverter(typeof(ConvertersIntId));
        var id = converter.ConvertFrom(value);
        Assert.IsType<ConvertersIntId>(id);
        Assert.Equal(new ConvertersIntId(123), id);

        var reconverted = converter.ConvertTo(id, value.GetType());
        Assert.Equal(value, reconverted);
    }

#if NET6_0_OR_GREATER
    [Fact]
    public void WhenConventionBasedEfCoreValueConverterUsesValueConverter_Id()
        => WhenConventionBasedEfCoreValueConverterUsesValueConverter(x => x.Entities,
            new TestEntity { Id = new ConvertersIntId(123) });

    [Fact]
    public void WhenConventionBasedEfCoreValueConverterUsesValueConverter_Converter()
        => WhenConventionBasedEfCoreValueConverterUsesValueConverter(c => c.Entities3,
            new TestEntity3 { Id = new IntId(123) });
        
    private void WhenConventionBasedEfCoreValueConverterUsesValueConverter<T>(Func<ConventionsDbContext, DbSet<T>> dbsetFunc, T entity) where T : class
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<ConventionsDbContext>()
            .UseSqlite(connection)
            .Options;

        using (var context = new ConventionsDbContext(options))
        {
            context.Database.EnsureCreated();
            dbsetFunc(context).Add(entity);
            context.SaveChanges();
        }

        using (var context = new ConventionsDbContext(options))
        {
            var all = dbsetFunc(context).ToList();
            Assert.Single(all);
        }
    }
#endif
#endregion


#region ConvertersIntId22
    [Fact]
    public void CanSerializeToInt_WithMultiTemplates_WithNewtonsoftJsonProvider()
    {
        var foo = new ConvertersIntId2(123);

        var serializedFoo = NewtonsoftJsonSerializer.SerializeObject(foo);
        var serializedInt = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

        Assert.Equal(serializedFoo, serializedInt);
    }

    [Fact]
    public void CanSerializeToInt_WithMultiTemplates_WithSystemTextJsonProvider()
    {
        var foo = new ConvertersIntId2(123);

        var serializedFoo = SystemTextJsonSerializer.Serialize(foo);
        var serializedInt = SystemTextJsonSerializer.Serialize(foo.Value);

        Assert.Equal(serializedFoo, serializedInt);
    }

    [Fact]
    public void CanRoundTripWhenInRecord()
    {
        var foo = new ToSerialize()
        {
            Id = new ConvertersIntId(123),
        };

        var serialized = SystemTextJsonSerializer.Serialize(foo);
        var deserialized = SystemTextJsonSerializer.Deserialize<ToSerialize>(serialized);
        Assert.Equal(foo, deserialized);

#if NET6_0_OR_GREATER
        serialized = SystemTextJsonSerializer.Serialize(foo, SystemTextJsonSerializerContext.Custom.IntIdTests);
        deserialized = SystemTextJsonSerializer.Deserialize<ToSerialize>(serialized, SystemTextJsonSerializerContext.Custom.IntIdTests);
        Assert.Equal(foo, deserialized);
#endif
    }

    [Fact]
    public void CanDeserializeFromInt_WithMultiTemplates_WithNewtonsoftJsonProvider()
    {
        var value = 123;
        var foo = new ConvertersIntId2(value);
        var serializedInt = NewtonsoftJsonSerializer.SerializeObject(value);

        var deserializedFoo = NewtonsoftJsonSerializer.DeserializeObject<ConvertersIntId2>(serializedInt);

        Assert.Equal(foo, deserializedFoo);
    }

    [Fact]
    public void CanDeserializeFromInt_WithMultiTemplates_WithSystemTextJsonProvider()
    {
        var value = 123;
        var foo = new ConvertersIntId2(value);
        var serializedInt = SystemTextJsonSerializer.Serialize(value);

        var deserializedFoo = SystemTextJsonSerializer.Deserialize<ConvertersIntId2>(serializedInt);

        Assert.Equal(foo, deserializedFoo);
    }

    [Fact]
    public void WhenEfCoreValueConverter_WithMultiTemplates_UsesValueConverter()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(connection)
            .Options;

        var original = new TestEntity2 {Id = new ConvertersIntId2(123)};
        using (var context = new TestDbContext(options))
        {
            context.Database.EnsureCreated();
            context.Entities2.Add(original);
            context.SaveChanges();
        }

        using (var context = new TestDbContext(options))
        {
            var all = context.Entities2.ToList();
            var retrieved = Assert.Single(all);
            Assert.Equal(original.Id, retrieved.Id);
        }
    }

    [Fact]
    public async Task WhenDapperValueConverter_WithMultiTemplates_UsesValueConverter()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var results = await connection.QueryAsync<ConvertersIntId2>("SELECT 123");

        var value = Assert.Single(results);
        Assert.Equal(new ConvertersIntId2(123), value);
    }

    [Theory]
    [InlineData(123)]
    [InlineData("123")]
    public void TypeConverter_WithMultiTemplates_CanConvertToAndFrom(object value)
    {
        var converter = TypeDescriptor.GetConverter(typeof(ConvertersIntId2));
        var id = converter.ConvertFrom(value);
        Assert.IsType<ConvertersIntId2>(id);
        Assert.Equal(new ConvertersIntId2(123), id);

        var reconverted = converter.ConvertTo(id, value.GetType());
        Assert.Equal(value, reconverted);
    }

#if NET6_0_OR_GREATER
    [Fact]
    public void WhenConventionBasedEfCoreValueConverter_WithMultiTemplates_UsesValueConverter()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<ConventionsDbContext>()
            .UseSqlite(connection)
            .Options;

        using (var context = new ConventionsDbContext(options))
        {
            context.Database.EnsureCreated();
            context.Entities2.Add(
                new TestEntity2 {Id = new ConvertersIntId2(123)});
            context.SaveChanges();
        }

        using (var context = new ConventionsDbContext(options))
        {
            var all = context.Entities2.ToList();
            Assert.Single(all);
        }
    }
#endif
#endregion

#if NET6_0_OR_GREATER
    internal class ConventionsDbContext : DbContext
    {
        public DbSet<TestEntity> Entities { get; set; }
        public DbSet<TestEntity2> Entities2 { get; set; }
        public DbSet<TestEntity3> Entities3 { get; set; }

        public ConventionsDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder
                .Properties<ConvertersIntId>()
                .HaveConversion<ConvertersIntId.EfCoreValueConverter>();
            configurationBuilder
                .Properties<ConvertersIntId2>()
                .HaveConversion<ConvertersIntId2.EfCoreValueConverter>();
            configurationBuilder
                .Properties<IntId>()
                .HaveConversion<IntConverters.EfCoreValueConverter>();
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
            modelBuilder
                .Entity<TestEntity2>(builder =>
                {
                    builder
                        .Property(x => x.Id)
                        .ValueGeneratedNever();
                });
            modelBuilder
                .Entity<TestEntity3>(builder =>
                {
                    builder
                        .Property(x => x.Id)
                        .ValueGeneratedNever();
                });
        }
    }
#endif

    internal class TestDbContext : DbContext
    {
        public DbSet<TestEntity> Entities { get; set; }
        public DbSet<TestEntity2> Entities2 { get; set; }

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
                        .HasConversion(new ConvertersIntId.EfCoreValueConverter())
                        .ValueGeneratedNever();
                });
            modelBuilder
                .Entity<TestEntity2>(builder =>
                {
                    builder
                        .Property(x => x.Id)
                        .HasConversion(new ConvertersIntId2.EfCoreValueConverter())
                        .ValueGeneratedNever();
                });
        }
    }

    internal class TestEntity
    {
        public ConvertersIntId Id { get; set; }
    }

    internal class EntityWithNullableId
    {
        public ConvertersIntId? Id { get; set; }
    }

    internal class TestEntity2
    {
        public ConvertersIntId2 Id { get; set; }
    }

    internal class TestEntity3
    {
        public IntId Id { get; set; }
    }

    internal class EntityWithNullableId2
    {
        public ConvertersIntId2? Id { get; set; }
    }

    internal class TypeWithDictionaryKeys
    {
        public Dictionary<IntId, string> Values { get; set; }
    }

    internal record ToSerialize
    {
        public ConvertersIntId Id { get; set; }
        public Guid Guid { get; set; } = Guid.NewGuid();
        public long Long { get; set; } = 123;
        public int Int { get; set; } = 456;
        public string String { get; set; } = "Something";
    }
}