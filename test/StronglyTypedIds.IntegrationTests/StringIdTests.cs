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
        public void CanSerializeToString_WithNewtonsoftJsonProvider()
        {
            var foo = new ConvertersStringId("123");

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
            Assert.Null(deserialize.Id);
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
        public async Task WhenDapperValueConverterUsesValueConverter()
        {
            using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();

            var results = await connection.QueryAsync<ConvertersStringId>("SELECT 'this is a value'");

            var value = Assert.Single(results);
            Assert.Equal(value, new ConvertersStringId("this is a value"));
        }

        [Theory]
        [InlineData("")]
        [InlineData("some value")]
        public void TypeConverter_CanConvertToAndFrom(object value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(ConvertersStringId));
            var id = converter.ConvertFrom(value);
            Assert.IsType<ConvertersStringId>(id);
            Assert.Equal(new ConvertersStringId(value?.ToString()), id);

            var reconverted = converter.ConvertTo(id, value.GetType());
            Assert.Equal(value, reconverted);
        }

        [Fact]
        public void CanCompareDefaults()
        {
            ConvertersStringId original = default;
            var other = ConvertersStringId.Empty;

            var compare1 = original.CompareTo(other);
            var compare2 = other.CompareTo(original);
            Assert.Equal(compare1, -compare2);
        }

        [Fact]
        public void CanEquateDefaults()
        {
            ConvertersStringId original = default;
            var other = ConvertersStringId.Empty;

            var equals1 = (original as IEquatable<ConvertersStringId>).Equals(other);
            var equals2 = (other as IEquatable<ConvertersStringId>).Equals(original);

            Assert.Equal(equals1, equals2);
        }

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


#if NET6_0_OR_GREATER
        [Fact]
        public void WhenConventionBasedEfCoreValueConverterUsesValueConverter()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<ConventionsDbContext>()
                .UseSqlite(connection)
                .Options;

            var original = new TestEntity { Id = Guid.NewGuid(), Name = new ConvertersStringId("some name") };
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
                    .Properties<ConvertersStringId>()
                    .HaveConversion<ConvertersStringId.EfCoreValueConverter>();
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
                            .HasConversion(new ConvertersStringId.EfCoreValueConverter())
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
    }
}