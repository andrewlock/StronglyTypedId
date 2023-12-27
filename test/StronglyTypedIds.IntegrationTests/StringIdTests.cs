using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public class StringIdTests
    {
        [Fact]
        public async Task ThrowsIfTryToCreateWithNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => Task.FromResult(new StringId(null)));
        }

        [Fact]
        public void SameValuesAreEqual()
        {
            var id = "some-value";
            var foo1 = new StringId(id);
            var foo2 = new StringId(id);

            Assert.Equal(foo1, foo2);
        }

        [Fact]
        public void EmptyValueIsEmpty()
        {
            Assert.Equal(StringId.Empty.Value, string.Empty);
        }

        [Fact]
        public void DifferentValuesAreUnequal()
        {
            var foo1 = new StringId("value1");
            var foo2 = new StringId("value2");

            Assert.NotEqual(foo1, foo2);
        }

        [Fact]
        public void OverloadsWorkCorrectly()
        {
            var id = "some-value";
            var same1 = new StringId(id);
            var same2 = new StringId(id);
            var different = new StringId("other value");

            Assert.True(same1 == same2);
            Assert.False(same1 == different);
            Assert.False(same1 != same2);
            Assert.True(same1 != different);
        }

        [Fact]
        public void DifferentTypesAreUnequal()
        {
            var bar = GuidId2.New();
            var foo = new StringId("Value");

            //Assert.NotEqual(bar, foo); // does not compile
            Assert.NotEqual((object)bar, (object)foo);
        }

        [Fact]
        public void CanSerializeToNullableId_WithNewtonsoftJsonProvider()
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

            var key = new StringId("78104553-f1cd-41ec-bcb6-d3a8ff8d994d");
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
        public void ImplementsInterfaces()
        {
            Assert.IsAssignableFrom<IEquatable<StringId>>(StringId.Empty);
            Assert.IsAssignableFrom<IComparable<StringId>>(StringId.Empty);

#pragma warning disable 184
#pragma warning disable CS0183
            Assert.True(StringId.Empty is IComparable<StringId>);
            Assert.True(StringId.Empty is IEquatable<StringId>);
#pragma warning restore CS0183
#pragma warning restore 184

#if NET6_0_OR_GREATER
            Assert.IsAssignableFrom<ISpanFormattable>(StringId.Empty);
#endif
#if NET7_0_OR_GREATER
            // doesn't compile if doesn't implement it 
            ParseAs<StringId>("123");
            ParseSpan<StringId>("123".AsSpan());

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

        [Theory]
        [InlineData("")]
        [InlineData("some value")]
        public void TypeConverter_CanConvertToAndFrom(object value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(StringId));
            var id = converter.ConvertFrom(value);
            Assert.IsType<StringId>(id);
            Assert.Equal(new StringId(value?.ToString()), id);

            var reconverted = converter.ConvertTo(id, value.GetType());
            Assert.Equal(value, reconverted);
        }

        [Fact]
        public void CanCompareDefaults()
        {
            StringId original = default;
            var other = StringId.Empty;

            var compare1 = original.CompareTo(other);
            var compare2 = other.CompareTo(original);
            Assert.Equal(compare1, -compare2);
        }

        [Fact]
        public void CanEquateDefaults()
        {
            StringId original = default;
            var other = StringId.Empty;

            var equals1 = (original as IEquatable<StringId>).Equals(other);
            var equals2 = (other as IEquatable<StringId>).Equals(original);

            Assert.Equal(equals1, equals2);
        }

#region ConvertersStringId
        [Fact]
        public void CanSerializeToString_WithNewtonsoftJsonProvider()
        {
            var foo = new ConvertersStringId("123");

            var serializedFoo = NewtonsoftJsonSerializer.SerializeObject(foo);
            var serializedString = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

            Assert.Equal(serializedFoo, serializedString);
        }

        [Fact]
        public void CanSerializeToString_WithSystemTextJsonProvider()
        {
            var foo = new ConvertersStringId("123");

            var serializedFoo = SystemTextJsonSerializer.Serialize(foo);
            var serializedString = SystemTextJsonSerializer.Serialize(foo.Value);

            Assert.Equal(serializedFoo, serializedString);
        }


        [Fact]
        public void CanRoundTripWhenInRecord()
        {
            var foo = new ToSerialize()
            {
                Id = new ConvertersStringId("123"),
            };

            var serialized = SystemTextJsonSerializer.Serialize(foo);
            var deserialized = SystemTextJsonSerializer.Deserialize<ToSerialize>(serialized);
            Assert.Equal(foo, deserialized);

#if NET6_0_OR_GREATER
            serialized = SystemTextJsonSerializer.Serialize(foo, SystemTextJsonSerializerContext.Custom.StringIdTests);
            deserialized = SystemTextJsonSerializer.Deserialize<ToSerialize>(serialized, SystemTextJsonSerializerContext.Custom.StringIdTests);
            Assert.Equal(foo, deserialized);
#endif
        }

        [Fact]
        public void CanDeserializeFromString_WithNewtonsoftJsonProvider()
        {
            var value = "123";
            var foo = new ConvertersStringId(value);
            var serializedString = NewtonsoftJsonSerializer.SerializeObject(value);

            var deserializedFoo = NewtonsoftJsonSerializer.DeserializeObject<ConvertersStringId>(serializedString);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanDeserializeFromString_WithSystemTextJsonProvider()
        {
            var value = "123";
            var foo = new ConvertersStringId(value);
            var serializedString = SystemTextJsonSerializer.Serialize(value);

            var deserializedFoo = SystemTextJsonSerializer.Deserialize<ConvertersStringId>(serializedString);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
        {
            var foo = new ConvertersStringId("123");

            var serialized = NewtonsoftJsonSerializer.SerializeObject(foo);

            var expected = "\"" + foo.Value + "\"";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenEfCoreValueConverterUsesValueConverter()
        {
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlite(connection)
                .Options;

            var original = new TestEntity { Id = Guid.NewGuid(), Name = new ConvertersStringId("some name") };
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
                Assert.Equal(original.Name, retrieved.Name);
            }
        }

        

        [Fact]
        public Task WhenDapperValueConverterUsesValueConverter_Id()
            => WhenDapperValueConverterUsesValueConverter(g => new ConvertersStringId(g));

        [Fact]
        public Task WhenDapperValueConverterUsesValueConverter_Converter()
            => WhenDapperValueConverterUsesValueConverter(g => new StringId(g));
        
        private async Task WhenDapperValueConverterUsesValueConverter<T>(Func<string, T> newFunc)
        {
            using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();

            var results = await connection.QueryAsync<T>("SELECT 'this is a value'");

            var value = Assert.Single(results);
            Assert.Equal(value, newFunc("this is a value"));
        }

#if NET6_0_OR_GREATER
        [Fact]
        public void WhenConventionBasedEfCoreValueConverterUsesValueConverter_Id()
            => WhenConventionBasedEfCoreValueConverterUsesValueConverter(x => x.Entities,
                new TestEntity { Id = Guid.NewGuid(), Name = new ConvertersStringId("some name") });

        [Fact]
        public void WhenConventionBasedEfCoreValueConverterUsesValueConverter_Converter()
            => WhenConventionBasedEfCoreValueConverterUsesValueConverter(c => c.Entities3,
                new TestEntity3 { Id = Guid.NewGuid(), Name = new StringId("some name") });
        
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
                var retrieved = Assert.Single(all);
            }
        }
#endif
#endregion

#region ConvertersStringId2
        [Fact]
        public void CanSerializeToString_WithMultiTemplates_WithNewtonsoftJsonProvider()
        {
            var foo = new ConvertersStringId2("123");

            var serializedFoo = NewtonsoftJsonSerializer.SerializeObject(foo);
            var serializedString = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

            Assert.Equal(serializedFoo, serializedString);
        }

        [Fact]
        public void CanSerializeToString_WithMultiTemplates_WithSystemTextJsonProvider()
        {
            var foo = new ConvertersStringId2("123");

            var serializedFoo = SystemTextJsonSerializer.Serialize(foo);
            var serializedString = SystemTextJsonSerializer.Serialize(foo.Value);

            Assert.Equal(serializedFoo, serializedString);
        }

        [Fact]
        public void CanDeserializeFromString_WithMultiTemplates_WithNewtonsoftJsonProvider()
        {
            var value = "123";
            var foo = new ConvertersStringId2(value);
            var serializedString = NewtonsoftJsonSerializer.SerializeObject(value);

            var deserializedFoo = NewtonsoftJsonSerializer.DeserializeObject<ConvertersStringId2>(serializedString);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanDeserializeFromString_WithMultiTemplates_WithSystemTextJsonProvider()
        {
            var value = "123";
            var foo = new ConvertersStringId2(value);
            var serializedString = SystemTextJsonSerializer.Serialize(value);

            var deserializedFoo = SystemTextJsonSerializer.Deserialize<ConvertersStringId2>(serializedString);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void WhenNoJsonConverter_WithMultiTemplates_NewtonsoftSerializesWithoutValueProperty()
        {
            var foo = new ConvertersStringId2("123");

            var serialized = NewtonsoftJsonSerializer.SerializeObject(foo);

            var expected = "\"" + foo.Value + "\"";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenEfCoreValueConverter_WithMultiTemplates_UsesValueConverter()
        {
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlite(connection)
                .Options;

            var original = new TestEntity2 { Id = Guid.NewGuid(), Name = new ConvertersStringId2("some name") };
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
                Assert.Equal(original.Name, retrieved.Name);
            }
        }

        [Fact]
        public async Task WhenDapperValueConverter_WithMultiTemplates_UsesValueConverter()
        {
            using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();

            var results = await connection.QueryAsync<ConvertersStringId2>("SELECT 'this is a value'");

            var value = Assert.Single(results);
            Assert.Equal(value, new ConvertersStringId2("this is a value"));
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

            var original = new TestEntity2 { Id = Guid.NewGuid(), Name = new ConvertersStringId2("some name") };
            using (var context = new ConventionsDbContext(options))
            {
                context.Database.EnsureCreated();
                context.Entities2.Add(original);
                context.SaveChanges();
            }

            using (var context = new ConventionsDbContext(options))
            {
                var all = context.Entities2.ToList();
                var retrieved = Assert.Single(all);
                Assert.Equal(original.Id, retrieved.Id);
                Assert.Equal(original.Name, retrieved.Name);
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
                    .Properties<ConvertersStringId>()
                    .HaveConversion<ConvertersStringId.EfCoreValueConverter>();
                configurationBuilder
                    .Properties<ConvertersStringId2>()
                    .HaveConversion<ConvertersStringId2.EfCoreValueConverter>();
                configurationBuilder
                    .Properties<StringId>()
                    .HaveConversion<StringConverters.EfCoreValueConverter>();
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
                            .Property(x => x.Name)
                            .HasConversion(new ConvertersStringId.EfCoreValueConverter())
                            .ValueGeneratedNever();
                    });
                modelBuilder
                    .Entity<TestEntity2>(builder =>
                    {
                        builder
                            .Property(x => x.Name)
                            .HasConversion(new ConvertersStringId2.EfCoreValueConverter())
                            .ValueGeneratedNever();
                    });
            }
        }

        internal class TestEntity
        {
            public Guid Id { get; set; }
            public ConvertersStringId Name { get; set; }
        }

        internal class EntityWithNullableId
        {
            public ConvertersStringId? Id { get; set; }
        }

        internal class TestEntity2
        {
            public Guid Id { get; set; }
            public ConvertersStringId2 Name { get; set; }
        }

        internal class TestEntity3
        {
            public Guid Id { get; set; }
            public StringId Name { get; set; }
        }

        internal class EntityWithNullableId2
        {
            public ConvertersStringId2? Id { get; set; }
        }

        internal class TypeWithDictionaryKeys
        {
            public Dictionary<StringId, string> Values { get; set; }
        }

        internal record ToSerialize
        {
            public ConvertersStringId Id { get; set; }
            public Guid Guid { get; set; } = Guid.NewGuid();
            public long Long { get; set; } = 123;
            public int Int { get; set; } = 456;
            public string String { get; set; } = "Something";
        }
    }
}