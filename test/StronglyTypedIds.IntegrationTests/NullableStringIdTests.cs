using System;
using System.ComponentModel;
using System.Linq;
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
            var foo = new NewtonsoftJsonNullableStringId("123");

            var serializedFoo = NewtonsoftJsonSerializer.SerializeObject(foo);
            var serializedString = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

            Assert.Equal(serializedFoo, serializedString);
        }

        [Fact]
        public void CanSerializeToString_WithSystemTextJsonProvider()
        {
            var foo = new SystemTextJsonNullableStringId("123");

            var serializedFoo = SystemTextJsonSerializer.Serialize(foo);
            var serializedString = SystemTextJsonSerializer.Serialize(foo.Value);

            Assert.Equal(serializedFoo, serializedString);
        }

        [Fact]
        public void CanDeserializeFromString_WithNewtonsoftJsonProvider()
        {
            var value = "123";
            var foo = new NewtonsoftJsonNullableStringId(value);
            var serializedString = NewtonsoftJsonSerializer.SerializeObject(value);

            var deserializedFoo = NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonNullableStringId>(serializedString);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanDeserializeFromString_WithSystemTextJsonProvider()
        {
            var value = "123";
            var foo = new SystemTextJsonNullableStringId(value);
            var serializedString = SystemTextJsonSerializer.Serialize(value);

            var deserializedFoo = SystemTextJsonSerializer.Deserialize<SystemTextJsonNullableStringId>(serializedString);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanSerializeToString_WithBothJsonConverters()
        {
            var foo = new BothJsonNullableStringId("123");

            var serializedFoo1 = NewtonsoftJsonSerializer.SerializeObject(foo);
            var serializedString1 = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

            var serializedFoo2 = SystemTextJsonSerializer.Serialize(foo);
            var serializedString2 = SystemTextJsonSerializer.Serialize(foo.Value);

            Assert.Equal(serializedFoo1, serializedString1);
            Assert.Equal(serializedFoo2, serializedString2);
        }

        [Fact]
        public void WhenNoJsonConverter_SystemTextJsonSerializesWithValueProperty()
        {
            var foo = new NoJsonNullableStringId("123");

            var serialized = SystemTextJsonSerializer.Serialize(foo);

            var expected = "{\"Value\":\"" + foo.Value + "\"}";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
        {
            var foo = new NoJsonNullableStringId("123");

            var serialized = NewtonsoftJsonSerializer.SerializeObject(foo);

            var expected = "\"" + foo.Value + "\"";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoTypeConverter_SerializesWithValueProperty()
        {
            var foo = new NoConvertersNullableStringId("123");

            var newtonsoft = SystemTextJsonSerializer.Serialize(foo);
            var systemText = SystemTextJsonSerializer.Serialize(foo);

            var expected = "{\"Value\":\"" + foo.Value + "\"}";

            Assert.Equal(expected, newtonsoft);
            Assert.Equal(expected, systemText);
        }

        [Fact]
        public void WhenEfCoreValueConverterUsesValueConverter()
        {
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlite(connection)
                .Options;

            var original = new TestEntity { Id = Guid.NewGuid(), Name = new EfCoreNullableStringId("some name") };
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

            var results = await connection.QueryAsync<DapperNullableStringId>("SELECT 'this is a value'");

            var value = Assert.Single(results);
            Assert.Equal(value, new DapperNullableStringId("this is a value"));
        }

        [Theory]
        [InlineData("")]
        [InlineData("some value")]
        public void TypeConverter_CanConvertToAndFrom(object value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(NoJsonNullableStringId));
            var id = converter.ConvertFrom(value);
            Assert.IsType<NoJsonNullableStringId>(id);
            Assert.Equal(new NoJsonNullableStringId(value?.ToString()), id);

            var reconverted = converter.ConvertTo(id, value.GetType());
            Assert.Equal(value, reconverted);
        }

        [Fact]
        public void CanCompareDefaults()
        {
            ComparableNullableStringId original = default;
            var other = ComparableNullableStringId.Empty;

            var compare1 = original.CompareTo(other);
            var compare2 = other.CompareTo(original);
            Assert.Equal(compare1, -compare2);
        }

        [Fact]
        public void CanEquateDefaults()
        {
            EquatableNullableStringId original = default;
            var other = EquatableNullableStringId.Empty;

            var equals1 = (original as IEquatable<EquatableNullableStringId>).Equals(other);
            var equals2 = (other as IEquatable<EquatableNullableStringId>).Equals(original);

            Assert.Equal(equals1, equals2);
        }

        [Fact]
        public void ImplementsInterfaces()
        {
            Assert.IsAssignableFrom<IEquatable<BothNullableStringId>>(BothNullableStringId.Empty);
            Assert.IsAssignableFrom<IComparable<BothNullableStringId>>(BothNullableStringId.Empty);

            Assert.IsAssignableFrom<IEquatable<EquatableNullableStringId>>(EquatableNullableStringId.Empty);
            Assert.IsAssignableFrom<IComparable<ComparableNullableStringId>>(ComparableNullableStringId.Empty);

#pragma warning disable 184
            Assert.False(NullableStringId.Empty is IComparable<NullableStringId>);
            Assert.False(NullableStringId.Empty is IEquatable<NullableStringId>);
#pragma warning restore 184
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
        public void NullSerializesAsExpectedWithoutConverters()
        {
            var expected = "{\"Value\":null}";
            var value = new NoConvertersNullableStringId(null);

            var json = SystemTextJsonSerializer.Serialize(value);
            var systemText = SystemTextJsonSerializer.Serialize(value);
            Assert.Equal(expected, json);
            Assert.Equal(expected, systemText);
        }

        [Fact]
        public void NullSerializesAsExpectedWithConverters()
        {
            var expected = "null";
            var value = new BothJsonNullableStringId(null);

            var json = SystemTextJsonSerializer.Serialize(value);
            var systemText = SystemTextJsonSerializer.Serialize(value);
            Assert.Equal(expected, systemText);
            Assert.Equal(expected, json);
        }

        [Fact]
        public void TypeConverterConvertsNullAsExpected()
        {
            var converter = TypeDescriptor.GetConverter(typeof(NoJsonNullableStringId));
            var id = converter.ConvertFrom(null);
            Assert.IsType<NoJsonNullableStringId>(id);
            Assert.Equal(new NoJsonNullableStringId(null), id);

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

            var original = new TestEntity { Id = Guid.NewGuid(), Name = new EfCoreNullableStringId("some name") };
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

        public class ConventionsDbContext : DbContext
        {
            public DbSet<TestEntity> Entities { get; set; }

            public ConventionsDbContext(DbContextOptions options) : base(options)
            {
            }

            protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
            {
                configurationBuilder
                    .Properties<EfCoreNullableStringId>()
                    .HaveConversion<EfCoreNullableStringId.EfCoreValueConverter>();
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
                            .Property(x => x.Name)
                            .HasConversion(new EfCoreNullableStringId.EfCoreValueConverter())
                            .ValueGeneratedNever();
                    });
            }
        }

        public class TestEntity
        {
            public Guid Id { get; set; }
            public EfCoreNullableStringId Name { get; set; }
        }
    }
}