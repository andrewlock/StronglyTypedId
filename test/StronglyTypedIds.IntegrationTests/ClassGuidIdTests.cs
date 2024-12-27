using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StronglyTypedIds.IntegrationTests.Types;
using Xunit;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace StronglyTypedIds.IntegrationTests;

public partial class ClassGuidIdTests
{
    [Fact]
    public void SameValuesAreEqual()
    {
        var id = Guid.NewGuid();
        var foo1 = new ClassGuidId(id);
        var foo2 = new ClassGuidId(id);

        Assert.Equal(foo1, foo2);
    }

    [Fact]
    public void EmptyValueIsEmpty()
    {
        Assert.Equal(ClassGuidId.Empty.Value, Guid.Empty);
    }

    [Fact]
    public void DifferentValuesAreUnequal()
    {
        var foo1 = ClassGuidId.New();
        var foo2 = ClassGuidId.New();

        Assert.NotEqual(foo1, foo2);
    }

    [Fact]
    public void OverloadsWorkCorrectly()
    {
        var id = Guid.NewGuid();
        var same1 = new ClassGuidId(id);
        var same2 = new ClassGuidId(id);
        var different = ClassGuidId.New();

        Assert.True(same1 == same2);
        Assert.False(same1 == different);
        Assert.False(same1 != same2);
        Assert.True(same1 != different);
    }

    [Fact]
    public void DifferentTypesAreUnequal()
    {
        var bar = GuidId2.New();
        var foo = ClassGuidId.New();

        //Assert.NotEqual(bar, foo); // does not compile
        Assert.NotEqual((object)bar, (object)foo);
    }

#if NET8_0_OR_GREATER
    [Fact]
    public void CanFormatAsUtf8()
    {
        var expected = "17b4c96b-023a-4050-8ab5-4511c0e7fd09"u8;
        var id = ClassGuidId.Parse("17B4C96B-023A-4050-8AB5-4511C0E7FD09");

        var format = "D"; // in expected format
        var actual = new byte[expected.Length].AsSpan();
        var success = id.TryFormat(actual, out var charsWritten, format, provider: null);
         
        Assert.True(success);
        Assert.Equal(expected.Length, charsWritten);
        Assert.True(actual.SequenceEqual(expected));
    }
#endif

    [Fact]
    public void CanSerializeToGuid_WithTypeConverter()
    {
        var foo = ClassGuidId.New();

        var serializedFoo = JsonConvert.SerializeObject(foo);
        var serializedGuid = JsonConvert.SerializeObject(foo.Value);

        Assert.Equal(serializedFoo, serializedGuid);
    }

    [Fact]
    public void CanSerializeToGuid_WithSystemTextJsonProvider()
    {
        var foo = ClassGuidId.New();

        var serializedFoo = JsonSerializer.Serialize(foo);
        var serializedGuid = JsonSerializer.Serialize(foo.Value);

        Assert.Equal(serializedFoo, serializedGuid);
    }

    [Fact]
    public void CanRoundTripWhenInRecord()
    {
        var foo = new ToSerialize()
        {
            Id = ClassGuidId.New(),
        };

        var serialized = JsonSerializer.Serialize(foo);
        var deserialized = JsonSerializer.Deserialize<ToSerialize>(serialized);
        Assert.Equal(foo, deserialized);
#if NET6_0_OR_GREATER
        serialized = JsonSerializer.Serialize(foo, SystemTextJsonSerializerContext.Custom.ClassGuidIdTests);
        deserialized = JsonSerializer.Deserialize<ToSerialize>(serialized, SystemTextJsonSerializerContext.Custom.ClassGuidIdTests);
        Assert.Equal(foo, deserialized);
#endif
    }

    [Fact]
    public void CanDeserializeFromGuid_WithSystemTextJsonProvider()
    {
        var value = Guid.NewGuid();
        var foo = new ClassGuidId(value);
        var serializedGuid = JsonSerializer.Serialize(value);

        var deserializedFoo = JsonSerializer.Deserialize<ClassGuidId>(serializedGuid);

        Assert.Equal(foo, deserializedFoo);
    }

#if NET6_0_OR_GREATER
    [Fact]
    public void CanDeserializeDictionaryKeys_WithSystemTextJsonProvider()
    {
        var value = new TypeWithDictionaryKeys()
        {
            Values = new()
        };
        var guid = new ClassGuidId(Guid.Parse("78104553-f1cd-41ec-bcb6-d3a8ff8d994d"));
        value.Values.Add(guid, "My Value");
        var opts = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        var serialized = JsonSerializer.Serialize(value, opts);

        var expected = $$"""
                         {
                           "values": {
                             "78104553-f1cd-41ec-bcb6-d3a8ff8d994d": "My Value"
                           }
                         }
                         """;
        Assert.Equal(serialized, expected);

        var deserialized = JsonSerializer.Deserialize<TypeWithDictionaryKeys>(serialized, opts);

        Assert.NotNull(deserialized.Values);
        Assert.True(deserialized.Values.ContainsKey(guid));
        Assert.Equal("My Value", deserialized.Values[guid]);
    }

    [Fact]
    public void CanSerializeToGuid_WithSystemTextJsonProvider_WithSourceGenerator()
    {
        var foo = ClassGuidId.New();

        var serializedFoo = JsonSerializer.Serialize(foo, SystemTextJsonSerializerContext.Custom.ClassGuidId);
        var serializedGuid = JsonSerializer.Serialize(foo.Value);

        Assert.Equal(serializedFoo, serializedGuid);
    }

    [Fact]
    public void CanDeserializeFromGuid_WithSystemTextJsonProvider_WithSourceGenerator()
    {
        var value = Guid.NewGuid();
        var foo = new ClassGuidId(value);
        var serializedGuid = JsonSerializer.Serialize(value);

        var deserializedFoo = JsonSerializer.Deserialize<ClassGuidId>(serializedGuid, SystemTextJsonSerializerContext.Custom.ClassGuidId);

        Assert.Equal(foo, deserializedFoo);
    }
#endif

    [Fact]
    public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
    {
        var foo = ClassGuidId.New();

        var serialized = JsonConvert.SerializeObject(foo);

        var expected =  $"\"{foo.Value}\"";

        Assert.Equal(expected, serialized);
    }

    [Theory]
    [InlineData("78104553-f1cd-41ec-bcb6-d3a8ff8d994d")]
    public void TypeConverter_CanConvertToAndFrom(string value)
    {
        var converter = TypeDescriptor.GetConverter(typeof(ClassGuidId));
        var id = converter.ConvertFrom(value);
        Assert.IsType<ClassGuidId>(id);
        Assert.Equal(new ClassGuidId(Guid.Parse(value)), id);

        var reconverted = converter.ConvertTo(id, value.GetType());
        Assert.Equal(value, reconverted);
    }

    [Fact]
    public void ImplementsInterfaces()
    {
        Assert.IsAssignableFrom<IEquatable<ClassGuidId>>(ClassGuidId.Empty);
        Assert.IsAssignableFrom<IComparable<ClassGuidId>>(ClassGuidId.Empty);
        Assert.IsAssignableFrom<IFormattable>(ClassGuidId.Empty);
            
#pragma warning disable CS0183
#pragma warning disable 184
        Assert.True(ClassGuidId.Empty is IComparable<ClassGuidId>);
        Assert.True(ClassGuidId.Empty is IEquatable<ClassGuidId>);
#pragma warning restore 184
#pragma warning restore CS0183

#if NET6_0_OR_GREATER
        Assert.IsAssignableFrom<ISpanFormattable>(ClassGuidId.Empty);
#endif
#if NET7_0_OR_GREATER
        // doesn't compile if doesn't implement it 
        ParseAs<ClassGuidId>(Guid.NewGuid().ToString());
        ParseSpan<ClassGuidId>(Guid.NewGuid().ToString().AsSpan());

        T ParseAs<T>(string s) where T: IParsable<T> {
            return T.Parse(s, null);
        }

        T ParseSpan<T>(ReadOnlySpan<char> s) where T: ISpanParsable<T> {
            return T.Parse(s, null);
        }
#endif
    }

    #region ConvertersGuidId       
    [Fact]
    public void CanDeserializeFromGuid_WithNewtonsoftJsonProvider()
    {
        var value = Guid.NewGuid();
        var foo = new ConvertersGuidId(value);
        var serializedGuid = JsonConvert.SerializeObject(value);
        
        var deserializedFoo = JsonConvert.DeserializeObject<ConvertersGuidId>(serializedGuid);
        
        Assert.Equal(foo, deserializedFoo);
    }

    [Fact]
    public void CanSerializeToNullableInt_WithNewtonsoftJsonProvider()
    {
        var entity = new EntityWithNullableId { Id = null };
        
        var json = JsonConvert.SerializeObject(entity);
        var deserialize = JsonConvert.DeserializeObject<EntityWithNullableId>(json);
        
        Assert.NotNull(deserialize);
        Assert.Null(deserialize.Id);
    }

    [Fact]
    public void CanSerializeToGuid_WithBothJsonConverters()
    {
        var foo = ConvertersGuidId.New();
        
        var serializedFoo1 = JsonConvert.SerializeObject(foo);
        var serializedGuid1 = JsonConvert.SerializeObject(foo.Value);
        
        var serializedFoo2 = JsonSerializer.Serialize(foo);
        var serializedGuid2 = JsonSerializer.Serialize(foo.Value);
        
        Assert.Equal(serializedFoo1, serializedGuid1);
        Assert.Equal(serializedFoo2, serializedGuid2);
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
                new TestEntity { Id = ConvertersGuidId.New() });
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
        
        var results = await connection.QueryAsync<ConvertersGuidId>("SELECT '5640dad4-862a-4738-9e3c-c76dc227eb66'");
        
        var value = Assert.Single(results);
        Assert.Equal(value, new ConvertersGuidId(Guid.Parse("5640dad4-862a-4738-9e3c-c76dc227eb66")));
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
                new TestEntity { Id = ConvertersGuidId.New() });
            context.SaveChanges();
        }
        using (var context = new ConventionsDbContext(options))
        {
            var all = context.Entities.ToList();
            Assert.Single(all);
        }
    }
#endif
    #endregion

    #region ConvertersGuidId2
    [Fact]
    public void CanDeserializeFromGuid_WithMultiTemplates_WithNewtonsoftJsonProvider()
    {
        var value = Guid.NewGuid();
        var foo = new ConvertersGuidId2(value);
        var serializedGuid = JsonConvert.SerializeObject(value);
        
        var deserializedFoo = JsonConvert.DeserializeObject<ConvertersGuidId2>(serializedGuid);
        
        Assert.Equal(foo, deserializedFoo);
    }

    [Fact]
    public void CanSerializeToNullable_WithMultiTemplates_WithNewtonsoftJsonProvider()
    {
        var entity = new EntityWithNullableId2 { Id = null };
        
        var json = JsonConvert.SerializeObject(entity);
        var deserialize = JsonConvert.DeserializeObject<EntityWithNullableId2>(json);
        
        Assert.NotNull(deserialize);
        Assert.Null(deserialize.Id);
    }

    [Fact]
    public void CanSerializeToGuid_WithMultiTemplates_WithBothJsonConverters()
    {
        var foo = ConvertersGuidId2.New();
        
        var serializedFoo1 = JsonConvert.SerializeObject(foo);
        var serializedGuid1 = JsonConvert.SerializeObject(foo.Value);
        
        var serializedFoo2 = JsonSerializer.Serialize(foo);
        var serializedGuid2 = JsonSerializer.Serialize(foo.Value);
        
        Assert.Equal(serializedFoo1, serializedGuid1);
        Assert.Equal(serializedFoo2, serializedGuid2);
    }

    [Fact]
    public void WhenEfCore_WithMultiTemplates_ValueConverterUsesValueConverter()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(connection)
            .Options;
        
        using (var context = new TestDbContext(options))
        {
            context.Database.EnsureCreated();
            context.Entities2.Add(
                new TestEntity2 { Id = ConvertersGuidId2.New() });
            context.SaveChanges();
        }
        using (var context = new TestDbContext(options))
        {
            var all = context.Entities2.ToList();
            Assert.Single(all);
        }
    }

    [Fact]
    public async Task WhenDapperValueConverter_WithMultiTemplates_UsesValueConverter()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        
        var results = await connection.QueryAsync<ConvertersGuidId2>("SELECT '5640dad4-862a-4738-9e3c-c76dc227eb66'");
        
        var value = Assert.Single(results);
        Assert.Equal(value, new ConvertersGuidId2(Guid.Parse("5640dad4-862a-4738-9e3c-c76dc227eb66")));
    }

#if NET6_0_OR_GREATER
    [Fact]
    public void WhenConventionBasedEfCore_WithMultiTemplates_ValueConverterUsesValueConverter()
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
                new TestEntity2 { Id = ConvertersGuidId2.New() });
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

        public ConventionsDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder
                .Properties<ConvertersGuidId>()
                .HaveConversion<ConvertersGuidId.EfCoreValueConverter>();
            configurationBuilder
                .Properties<ConvertersGuidId2>()
                .HaveConversion<ConvertersGuidId2.EfCoreValueConverter>();
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
                        .HasConversion(new ConvertersGuidId.EfCoreValueConverter())
                        .ValueGeneratedNever();
                });

            modelBuilder
                .Entity<TestEntity2>(builder =>
                {
                    builder
                        .Property(x => x.Id)
                        .HasConversion(new ConvertersGuidId2.EfCoreValueConverter())
                        .ValueGeneratedNever();
                });
        }
    }

    internal class TestEntity
    {
        public ConvertersGuidId Id { get; set; }
    }

    internal class EntityWithNullableId
    {
        public ConvertersGuidId? Id { get; set; }
    }

    internal class TypeWithDictionaryKeys
    {
        public Dictionary<ClassGuidId, string> Values { get; set; }
    }

    internal class TestEntity2
    {
        public ConvertersGuidId2 Id { get; set; }
    }

    internal class EntityWithNullableId2
    {
        public ConvertersGuidId2? Id { get; set; }
    }

    internal record ToSerialize
    {
        public ClassGuidId Id { get; set; }
        public Guid Guid { get; set; } = Guid.NewGuid();
        public long Long { get; set; } = 123;
        public int Int { get; set; } = 456;
        public string String { get; set; } = "Something";
    }
}