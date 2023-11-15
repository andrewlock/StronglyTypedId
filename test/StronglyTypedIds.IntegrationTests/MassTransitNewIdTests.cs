using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Dapper;
using MassTransit;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using StronglyTypedIds.IntegrationTests.Types;
using Xunit;
using NewtonsoftJsonSerializer = Newtonsoft.Json.JsonConvert;
using SystemTextJsonSerializer = System.Text.Json.JsonSerializer;

namespace StronglyTypedIds.IntegrationTests
{
    public class MassTransitNewIdTests
    {
        [Fact]
        public void SameValuesAreEqual()
        {
            var id = NewId.Next();
            var foo1 = new NewIdId1(id);
            var foo2 = new NewIdId1(id);

            Assert.Equal(foo1, foo2);
        }

        [Fact]
        public void EmptyValueIsEmpty()
        {
            Assert.Equal(NewIdId1.Empty.Value, NewId.Empty);
        }


        [Fact]
        public void DifferentValuesAreUnequal()
        {
            var foo1 = NewIdId1.New();
            var foo2 = NewIdId1.New();

            Assert.NotEqual(foo1, foo2);
        }

        [Fact]
        public void OverloadsWorkCorrectly()
        {
            var id = NewId.Next();
            var same1 = new NewIdId1(id);
            var same2 = new NewIdId1(id);
            var different = NewIdId1.New();

            Assert.True(same1 == same2);
            Assert.False(same1 == different);
            Assert.False(same1 != same2);
            Assert.True(same1 != different);
        }

        [Fact]
        public void DifferentTypesAreUnequal()
        {
            var bar = NewIdId2.New();
            var foo = NewIdId1.New();

            //Assert.NotEqual(bar, foo); // does not compile
            Assert.NotEqual((object)bar, (object)foo);
        }

        [Fact]
        public void CantCreateEmptyGeneratedId1()
        {
            var foo = new NewIdId1();
            var bar = new NewIdId2();

            //Assert.NotEqual(bar, foo); // does not compile
            Assert.NotEqual((object)bar, (object)foo);
        }

        [Fact]
        public void CanSerializeToNewId_WithSystemTextJsonProvider()
        {
            var foo = NewIdId1.New();

            var serializedFoo = SystemTextJsonSerializer.Serialize(foo);
            var serializedNewId = SystemTextJsonSerializer.Serialize(foo.Value.ToGuid().ToString());

            Assert.Equal(serializedFoo, serializedNewId);
        }

#if NET6_0_OR_GREATER
        [Fact]
        public void CanDeserializeDictionaryKeys_WithSystemTextJsonProvider()
        {
            var value = new TypeWithDictionaryKeys()
            {
                Values = new()
            };

            var key = new NewIdId1(NewId.FromGuid(Guid.Parse("78104553-f1cd-41ec-bcb6-d3a8ff8d994d")));
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
                                 "78104553-f1cd-41ec-bcb6-d3a8ff8d994d": "My Value"
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
        public void CanDeserializeFromNewId_WithNewtonsoftJsonProvider()
        {
            var value = NewId.Next();
            var foo = new NewIdId1(value);
            var serializedNewId = NewtonsoftJsonSerializer.SerializeObject(value.ToGuid());

            var deserializedFoo = NewtonsoftJsonSerializer.DeserializeObject<NewIdId1>(serializedNewId);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanSerializeToNullableInt_WithNewtonsoftJsonProvider()
        {
            var entity = new EntityWithNullableId { Id = null };

            var json = NewtonsoftJsonSerializer.SerializeObject(entity);
            var deserialize = NewtonsoftJsonSerializer.DeserializeObject<EntityWithNullableId>(json);

            Assert.NotNull(deserialize);
            Assert.Null(deserialize.Id);
        }

        [Fact]
        public void CanDeserializeFromNewId_WithSystemTextJsonProvider()
        {
            var value = NewId.Next();
            var foo = new NewIdId1(value);
            var serializedNewId = SystemTextJsonSerializer.Serialize(value.ToGuid());

            var deserializedFoo = SystemTextJsonSerializer.Deserialize<NewIdId1>(serializedNewId);

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

            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                context.Entities.Add(
                    new TestEntity { Id = NewIdId1.New() });
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

            var results = await connection.QueryAsync<NewIdId1>("SELECT '5640dad4-862a-4738-9e3c-c76dc227eb66'");

            var value = Assert.Single(results);
            Assert.Equal(new NewIdId1(NewId.FromGuid(Guid.Parse("5640dad4-862a-4738-9e3c-c76dc227eb66"))), value);
        }

        [Theory]
        [InlineData("78104553-f1cd-41ec-bcb6-d3a8ff8d994d")]
        public void TypeConverter_CanConvertToAndFrom(string value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(NewIdId1));
            var id = converter.ConvertFrom(value);
            Assert.IsType<NewIdId1>(id);
            Assert.Equal(new NewIdId1(NewId.FromGuid(Guid.Parse(value))), id);

            var reconverted = converter.ConvertTo(id, value.GetType());
            Assert.Equal(value, reconverted);
        }

        [Fact]
        public void CanCompareDefaults()
        {
            NewIdId1 original = default;
            var other = NewIdId1.Empty;

            var compare1 = original.CompareTo(other);
            var compare2 = other.CompareTo(original);
            Assert.Equal(compare1, -compare2);
        }

        [Fact]
        public void CanEquateDefaults()
        {
            NewIdId1 original = default;
            var other = NewIdId1.Empty;

            var equals1 = (original as IEquatable<NewIdId1>).Equals(other);
            var equals2 = (other as IEquatable<NewIdId1>).Equals(original);

            Assert.Equal(equals1, equals2);
        }

        [Fact]
        public void ImplementsInterfaces()
        {
            Assert.IsAssignableFrom<IEquatable<NewIdId1>>(NewIdId1.Empty);
            Assert.IsAssignableFrom<IComparable<NewIdId1>>(NewIdId1.Empty);

#pragma warning disable CS0183
#pragma warning disable 184
            Assert.True(NewIdId1.Empty is IComparable<NewIdId1>);
            Assert.True(NewIdId1.Empty is IEquatable<NewIdId1>);
#pragma warning restore 184
#pragma warning restore CS0183

#if NET6_0_OR_GREATER
            Assert.IsAssignableFrom<ISpanFormattable>(NewIdId1.Empty);
#endif
#if NET7_0_OR_GREATER
            // doesn't compile if doesn't implement it 
            ParseAs<NewIdId1>(Guid.NewGuid().ToString());
            ParseSpan<NewIdId1>(Guid.NewGuid().ToString().AsSpan());

            T ParseAs<T>(string s) where T: IParsable<T> {
                return T.Parse(s, null);
            }

            T ParseSpan<T>(ReadOnlySpan<char> s) where T: ISpanParsable<T> {
                return T.Parse(s, null);
            }
#endif
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
                    new TestEntity { Id = NewIdId1.New() });
                context.SaveChanges();
            }
            using (var context = new ConventionsDbContext(options))
            {
                var all = context.Entities.ToList();
                Assert.Single(all);
            }
        }

        internal class ConventionsDbContext : DbContext
        {
            public DbSet<TestEntity> Entities { get; set; }

            public ConventionsDbContext(DbContextOptions options) : base(options)
            {
            }

            protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
            {
                configurationBuilder
                    .Properties<NewIdId1>()
                    .HaveConversion<NewIdId1.EfCoreValueConverter>();
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

        internal class TestDbContext : DbContext
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
                            .HasConversion(new NewIdId1.EfCoreValueConverter())
                            .ValueGeneratedNever();
                    });
            }
        }

        internal class TestEntity
        {
            public NewIdId1 Id { get; set; }
        }

        internal class EntityWithNullableId
        {
            public NewIdId1? Id { get; set; }
        }

        internal class TypeWithDictionaryKeys
        {
            public Dictionary<NewIdId1, string> Values { get; set; }
        }
    }
}