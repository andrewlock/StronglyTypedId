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

namespace StronglyTypedIds.IntegrationTests
{
    public partial class LongIdTests
    {
        [Fact]
        public void SameValuesAreEqual()
        {
            var id = 123L;
            var foo1 = new LongId(id);
            var foo2 = new LongId(id);

            Assert.Equal(foo1, foo2);
        }

        [Fact]
        public void EmptyValueIsEmpty()
        {
            Assert.Equal(0, LongId.Empty.Value);
        }

        [Fact]
        public void DifferentValuesAreUnequal()
        {
            var foo1 = new LongId(1L);
            var foo2 = new LongId(2L);

            Assert.NotEqual(foo1, foo2);
        }

        [Fact]
        public void OverloadsWorkCorrectly()
        {
            var id = 12L;
            var same1 = new LongId(id);
            var same2 = new LongId(id);
            var different = new LongId(3L);

            Assert.True(same1 == same2);
            Assert.False(same1 == different);
            Assert.False(same1 != same2);
            Assert.True(same1 != different);
        }

        [Fact]
        public void DifferentTypesAreUnequal()
        {
            var bar = GuidId2.New();
            var foo = new LongId(23L);

            //Assert.NotEqual(bar, foo); // does not compile
            Assert.NotEqual((object)bar, (object)foo);
        }

#if NET8_0_OR_GREATER
        [Fact]
        public void CanRoundTripUtf8()
        {
            var id = new LongId(123L);

            var actual = new byte[16].AsSpan();
            Assert.True(id.TryFormat(actual, out var charsWritten));

            var success = LongId.TryParse(actual.Slice(0, charsWritten), provider: null, out var result);

            Assert.True(success);
            Assert.Equal(id, result);
        }
#endif

        [Fact]
        public void CanSerializeToNullableInt_WithNewtonsoftJsonProvider()
        {
            var entity = new EntityWithNullableId { Id = null };

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
            var key = new LongId(123);
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
            var foo = new LongId(123);

            var serialized = NewtonsoftJsonSerializer.SerializeObject(foo);

            var expected = $"\"{foo.Value}\"";

            Assert.Equal(expected, serialized);
        }

        [Theory]
        [InlineData(123L)]
        [InlineData("123")]
        public void TypeConverter_CanConvertToAndFrom(object value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(LongId));
            var id = converter.ConvertFrom(value);
            Assert.IsType<LongId>(id);
            Assert.Equal(new LongId(123), id);

            var reconverted = converter.ConvertTo(id, value.GetType());
            Assert.Equal(value, reconverted);
        }

        [Fact]
        public void CanCompareDefaults()
        {
            LongId original = default;
            var other = LongId.Empty;

            var compare1 = original.CompareTo(other);
            var compare2 = other.CompareTo(original);
            Assert.Equal(compare1, -compare2);
        }

        [Fact]
        public void CanEquateDefaults()
        {
            LongId original = default;
            var other = LongId.Empty;

            var equals1 = (original as IEquatable<LongId>).Equals(other);
            var equals2 = (other as IEquatable<LongId>).Equals(original);

            Assert.Equal(equals1, equals2);
        }

        [Fact]
        public void ImplementsInterfaces()
        {
            Assert.IsAssignableFrom<IEquatable<LongId>>(LongId.Empty);
            Assert.IsAssignableFrom<IComparable<LongId>>(LongId.Empty);

#pragma warning disable 184
#pragma warning disable CS0183
            Assert.True(LongId.Empty is IComparable<LongId>);
            Assert.True(LongId.Empty is IEquatable<LongId>);
#pragma warning restore CS0183
#pragma warning restore 184
            
#if NET6_0_OR_GREATER
            Assert.IsAssignableFrom<ISpanFormattable>(LongId.Empty);
#endif
#if NET7_0_OR_GREATER
            // doesn't compile if doesn't implement it 
            ParseAs<LongId>("123");
            ParseSpan<LongId>("123".AsSpan());

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

#region ConvertersLongId
        [Fact]
        public void CanSerializeToLong_WithNewtonsoftJsonProvider()
        {
            var foo = new ConvertersLongId(123);

            var serializedFoo = NewtonsoftJsonSerializer.SerializeObject(foo);
            var serializedLong = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

            Assert.Equal(serializedFoo, serializedLong);
        }

        [Fact]
        public void CanSerializeToLong_WithSystemTextJsonProvider()
        {
            var foo = new ConvertersLongId(123L);

            var serializedFoo = SystemTextJsonSerializer.Serialize(foo);
            var serializedLong = SystemTextJsonSerializer.Serialize(foo.Value);

            Assert.Equal(serializedFoo, serializedLong);
        }

        [Fact]
        public void CanDeserializeFromLong_WithNewtonsoftJsonProvider()
        {
            var value = 123L;
            var foo = new ConvertersLongId(value);
            var serializedLong = NewtonsoftJsonSerializer.SerializeObject(value);

            var deserializedFoo = NewtonsoftJsonSerializer.DeserializeObject<ConvertersLongId>(serializedLong);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanDeserializeFromLong_WithSystemTextJsonProvider()
        {
            var value = 123L;
            var foo = new ConvertersLongId(value);
            var serializedLong = SystemTextJsonSerializer.Serialize(value);

            var deserializedFoo = SystemTextJsonSerializer.Deserialize<ConvertersLongId>(serializedLong);

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

            var original = new TestEntity { Id = new ConvertersLongId(123) };
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
        public async Task WhenDapperValueConverterUsesValueConverter()
        {
            using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();

            var results = await connection.QueryAsync<ConvertersLongId>("SELECT 123");

            var value = Assert.Single(results);
            Assert.Equal(value, new ConvertersLongId(123));
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
            var handler = new ConvertersLongId.DapperTypeHandler();
            var value = handler.Parse((decimal) 123L);

            Assert.Equal(new ConvertersLongId(123), value);
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
                    new TestEntity { Id = new ConvertersLongId(123) });
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


#region ConvertersLongId2
        [Fact]
        public void CanSerializeToLong_WithMultiTemplates_WithNewtonsoftJsonProvider()
        {
            var foo = new ConvertersLongId2(123);

            var serializedFoo = NewtonsoftJsonSerializer.SerializeObject(foo);
            var serializedLong = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

            Assert.Equal(serializedFoo, serializedLong);
        }

        [Fact]
        public void CanSerializeToLong_WithMultiTemplates_WithSystemTextJsonProvider()
        {
            var foo = new ConvertersLongId2(123L);

            var serializedFoo = SystemTextJsonSerializer.Serialize(foo);
            var serializedLong = SystemTextJsonSerializer.Serialize(foo.Value);

            Assert.Equal(serializedFoo, serializedLong);
        }

        [Fact]
        public void CanRoundTripWhenInRecord()
        {
            var foo = new ToSerialize()
            {
                Id = new ConvertersLongId(123),
            };

            var serialized = SystemTextJsonSerializer.Serialize(foo);
            var deserialized = SystemTextJsonSerializer.Deserialize<ToSerialize>(serialized);
            Assert.Equal(foo, deserialized);

#if NET6_0_OR_GREATER
            serialized = SystemTextJsonSerializer.Serialize(foo, SystemTextJsonSerializerContext.Custom.LongIdTests);
            deserialized = SystemTextJsonSerializer.Deserialize<ToSerialize>(serialized, SystemTextJsonSerializerContext.Custom.LongIdTests);
            Assert.Equal(foo, deserialized);
#endif
        }

        [Fact]
        public void CanDeserializeFromLong_WithMultiTemplates_WithNewtonsoftJsonProvider()
        {
            var value = 123L;
            var foo = new ConvertersLongId2(value);
            var serializedLong = NewtonsoftJsonSerializer.SerializeObject(value);

            var deserializedFoo = NewtonsoftJsonSerializer.DeserializeObject<ConvertersLongId2>(serializedLong);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanDeserializeFromLong_WithMultiTemplates_WithSystemTextJsonProvider()
        {
            var value = 123L;
            var foo = new ConvertersLongId2(value);
            var serializedLong = SystemTextJsonSerializer.Serialize(value);

            var deserializedFoo = SystemTextJsonSerializer.Deserialize<ConvertersLongId2>(serializedLong);

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

            var original = new TestEntity2 { Id = new ConvertersLongId2(123) };
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

            var results = await connection.QueryAsync<ConvertersLongId2>("SELECT 123");

            var value = Assert.Single(results);
            Assert.Equal(value, new ConvertersLongId2(123));
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
                    new TestEntity2 { Id = new ConvertersLongId2(123) });
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
                    .Properties<ConvertersLongId>()
                    .HaveConversion<ConvertersLongId.EfCoreValueConverter>();
                configurationBuilder
                    .Properties<ConvertersLongId2>()
                    .HaveConversion<ConvertersLongId2.EfCoreValueConverter>();
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
                            .HasConversion(new ConvertersLongId.EfCoreValueConverter())
                            .ValueGeneratedNever();
                    });
                modelBuilder
                    .Entity<TestEntity2>(builder =>
                    {
                        builder
                            .Property(x => x.Id)
                            .HasConversion(new ConvertersLongId2.EfCoreValueConverter())
                            .ValueGeneratedNever();
                    });
            }
        }

        internal class TestEntity
        {
            public ConvertersLongId Id { get; set; }
        }

        internal class EntityWithNullableId
        {
            public ConvertersLongId? Id { get; set; }
        }

        internal class TestEntity2
        {
            public ConvertersLongId2 Id { get; set; }
        }

        internal class EntityWithNullableId2
        {
            public ConvertersLongId2? Id { get; set; }
        }

        internal class TypeWithDictionaryKeys
        {
            public Dictionary<LongId, string> Values { get; set; }
        }

        internal record ToSerialize
        {
            public ConvertersLongId Id { get; set; }
            public Guid Guid { get; set; } = Guid.NewGuid();
            public long Long { get; set; } = 123;
            public int Int { get; set; } = 456;
            public string String { get; set; } = "Something";
        }
    }
}