using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
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
    public class NullableStringIdTests
    {
        [Fact]
        public void CanUseNull()
        {
            var foo1 = new NullableStringId(null);

            Assert.Null(foo1.Value);
        }

        [Fact]
        public void SameValuesAreEqual()
        {
            var id = "some-value";
            var foo1 = new NullableStringId(id);
            var foo2 = new NullableStringId(id);

            Assert.Equal(foo1, foo2);
        }

        [Fact]
        public void EmptyValueIsEmpty()
        {
            Assert.Equal(NullableStringId.Empty.Value, string.Empty);
        }


        [Fact]
        public void DifferentValuesAreUnequal()
        {
            var foo1 = new NullableStringId("value1");
            var foo2 = new NullableStringId("value2");

            Assert.NotEqual(foo1, foo2);
        }

        [Fact]
        public void OverloadsWorkCorrectly()
        {
            var id = "some-value";
            var same1 = new NullableStringId(id);
            var same2 = new NullableStringId(id);
            var different = new NullableStringId("other value");

            Assert.True(same1 == same2);
            Assert.False(same1 == different);
            Assert.False(same1 != same2);
            Assert.True(same1 != different);
        }

        [Fact]
        public void DifferentTypesAreUnequal()
        {
            var bar = GuidId2.New();
            var foo = new NullableStringId("Value");

            //Assert.NotEqual(bar, foo); // does not compile
            Assert.NotEqual((object)bar, (object)foo);
        }

        [Fact]
        public void CanSerializeToString_WithNewtonsoftJsonProvider()
        {
            var foo = new NullableStringId("123");

            var serializedFoo = NewtonsoftJsonSerializer.SerializeObject(foo);
            var serializedString = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

            Assert.Equal(serializedFoo, serializedString);
        }

        [Fact]
        public void CanSerializeToNullableId_WithNewtonsoftJsonProvider()
        {
            var entity = new EntityWithNullableId { Id = null };

            var json = NewtonsoftJsonSerializer.SerializeObject(entity);
            var deserialize = NewtonsoftJsonSerializer.DeserializeObject<EntityWithNullableId>(json);

            Assert.NotNull(deserialize);
            // Note the different behaviour from String ID - this will _always_ deserialize to an ID
            Assert.NotNull(deserialize.Id);
            Assert.Null(deserialize.Id.Value.Value);
        }

        [Fact]
        public void CanSerializeToString_WithSystemTextJsonProvider()
        {
            var foo = new NullableStringId("123");

            var serializedFoo = SystemTextJsonSerializer.Serialize(foo);
            var serializedString = SystemTextJsonSerializer.Serialize(foo.Value);

            Assert.Equal(serializedFoo, serializedString);
        }

#if NET6_0_OR_GREATER
        [Fact]
        public void CanDeserializeDictionaryKeys_WithSystemTextJsonProvider()
        {
            var value = new TypeWithDictionaryKeys()
            {
                Values = new()
            };

            var key = new NullableStringId("78104553-f1cd-41ec-bcb6-d3a8ff8d994d");
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
        public void CanDeserializeFromString_WithNewtonsoftJsonProvider()
        {
            var value = "123";
            var foo = new NullableStringId(value);
            var serializedString = NewtonsoftJsonSerializer.SerializeObject(value);

            var deserializedFoo = NewtonsoftJsonSerializer.DeserializeObject<NullableStringId>(serializedString);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanDeserializeFromString_WithSystemTextJsonProvider()
        {
            var value = "123";
            var foo = new NullableStringId(value);
            var serializedString = SystemTextJsonSerializer.Serialize(value);

            var deserializedFoo = SystemTextJsonSerializer.Deserialize<NullableStringId>(serializedString);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
        {
            var foo = new NullableStringId("123");

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

            var original = new TestEntity { Id = Guid.NewGuid(), Name = new NullableStringId("some name") };
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
        public async Task WhenDapperValueConverterUsesValueConverter()
        {
            using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();

            var results = await connection.QueryAsync<NullableStringId>("SELECT 'this is a value'");

            var value = Assert.Single(results);
            Assert.Equal(value, new NullableStringId("this is a value"));
        }

        [Theory]
        [InlineData("")]
        [InlineData("some value")]
        public void TypeConverter_CanConvertToAndFrom(object value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(NullableStringId));
            var id = converter.ConvertFrom(value);
            Assert.IsType<NullableStringId>(id);
            Assert.Equal(new NullableStringId(value?.ToString()), id);

            var reconverted = converter.ConvertTo(id, value.GetType());
            Assert.Equal(value, reconverted);
        }

        [Fact]
        public void CanCompareDefaults()
        {
            NullableStringId original = default;
            var other = NullableStringId.Empty;

            var compare1 = original.CompareTo(other);
            var compare2 = other.CompareTo(original);
            Assert.Equal(compare1, -compare2);
        }

        [Fact]
        public void CanEquateDefaults()
        {
            NullableStringId original = default;
            var other = NullableStringId.Empty;

            var equals1 = (original as IEquatable<NullableStringId>).Equals(other);
            var equals2 = (other as IEquatable<NullableStringId>).Equals(original);

            Assert.Equal(equals1, equals2);
        }

        [Fact]
        public void ImplementsInterfaces()
        {
            Assert.IsAssignableFrom<IEquatable<NullableStringId>>(NullableStringId.Empty);
            Assert.IsAssignableFrom<IComparable<NullableStringId>>(NullableStringId.Empty);

#pragma warning disable 184
#pragma warning disable CS0183
            Assert.True(NullableStringId.Empty is IComparable<NullableStringId>);
            Assert.True(NullableStringId.Empty is IEquatable<NullableStringId>);
#pragma warning restore CS0183
#pragma warning restore 184

#if NET6_0_OR_GREATER
            Assert.IsAssignableFrom<ISpanFormattable>(NullableStringId.Empty);
#endif
#if NET7_0_OR_GREATER
            // doesn't compile if doesn't implement it 
            ParseAs<NullableStringId>("123");
            ParseSpan<NullableStringId>("123".AsSpan());

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

        [Fact]
        public void NullValueIsNull()
        {
            var value = new NullableStringId(null);
            Assert.Null(value.Value);
        }

        [Fact]
        public void CanEquateNullableValues()
        {
            var value1 = new NullableStringId(null);
            var value2 = new NullableStringId(null);
            var value3 = new NullableStringId(string.Empty);

            Assert.True(value1.Equals(value2));
            Assert.True(value1.Equals((object)value2));

            Assert.False(value1.Equals(value3));
            Assert.False(value1.Equals((object)value3));
            Assert.False(value3.Equals((object)value1));
        }

        [Fact]
        public void NullHashCodes()
        {
            var value1 = new NullableStringId(null);
            var value2 = new NullableStringId(null);
            var value3 = new NullableStringId("something");
            var value4 = new NullableStringId(string.Empty);

            Assert.Equal(value1.GetHashCode(), value2.GetHashCode());
            Assert.NotEqual(value1.GetHashCode(), value3.GetHashCode());
            Assert.NotEqual(value1.GetHashCode(), value4.GetHashCode());
        }

        [Fact]
        public void NullSerializesAsExpectedWithConverters()
        {
            var expected = "null";
            var value = new NullableStringId(null);

            var json = SystemTextJsonSerializer.Serialize(value);
            var systemText = SystemTextJsonSerializer.Serialize(value);
            Assert.Equal(expected, systemText);
            Assert.Equal(expected, json);
        }

        [Fact]
        public void TypeConverterConvertsNullAsExpected()
        {
            var converter = TypeDescriptor.GetConverter(typeof(NullableStringId));
            var id = converter.ConvertFrom(null);
            Assert.IsType<NullableStringId>(id);
            Assert.Equal(new NullableStringId(null), id);

            var reconverted = converter.ConvertTo(id, typeof(string));
            Assert.Null(reconverted);
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

            var original = new TestEntity { Id = Guid.NewGuid(), Name = new NullableStringId("some name") };
            using (var context = new ConventionsDbContext(options))
            {
                context.Database.EnsureCreated();
                context.Entities.Add(original);
                context.SaveChanges();
            }

            using (var context = new ConventionsDbContext(options))
            {
                var all = context.Entities.ToList();
                var retrieved = Assert.Single(all);
                Assert.Equal(original.Id, retrieved.Id);
                Assert.Equal(original.Name, retrieved.Name);
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
                    .Properties<NullableStringId>()
                    .HaveConversion<NullableStringId.EfCoreValueConverter>();
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
                            .Property(x => x.Name)
                            .HasConversion(new NullableStringId.EfCoreValueConverter())
                            .ValueGeneratedNever();
                    });
            }
        }

        internal class TestEntity
        {
            public Guid Id { get; set; }
            public NullableStringId Name { get; set; }
        }
        
        
        internal class EntityWithNullableId
        {
            public NullableStringId? Id { get; set; }
        }

        internal class TypeWithDictionaryKeys
        {
            public Dictionary<NullableStringId, string> Values { get; set; }
        }
    }
}