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
    public class GuidIdTests
    {
        [Fact]
        public void SameValuesAreEqual()
        {
            var id = Guid.NewGuid();
            var foo1 = new GuidId1(id);
            var foo2 = new GuidId1(id);

            Assert.Equal(foo1, foo2);
        }

        [Fact]
        public void EmptyValueIsEmpty()
        {
            Assert.Equal(GuidId1.Empty.Value, Guid.Empty);
        }


        [Fact]
        public void DifferentValuesAreUnequal()
        {
            var foo1 = GuidId1.New();
            var foo2 = GuidId1.New();

            Assert.NotEqual(foo1, foo2);
        }

        [Fact]
        public void OverloadsWorkCorrectly()
        {
            var id = Guid.NewGuid();
            var same1 = new GuidId1(id);
            var same2 = new GuidId1(id);
            var different = GuidId1.New();

            Assert.True(same1 == same2);
            Assert.False(same1 == different);
            Assert.False(same1 != same2);
            Assert.True(same1 != different);
        }

        [Fact]
        public void DifferentTypesAreUnequal()
        {
            var bar = GuidId2.New();
            var foo = GuidId1.New();

            //Assert.NotEqual(bar, foo); // does not compile
            Assert.NotEqual((object)bar, (object)foo);
        }

        [Fact]
        public void CantCreateEmptyGeneratedId1()
        {
            var foo = new GuidId1();
            var bar = new GuidId2();

            //Assert.NotEqual(bar, foo); // does not compile
            Assert.NotEqual((object)bar, (object)foo);
        }

        [Fact]
        public void CanSerializeToGuid_WithTypeConverter()
        {
            var foo = NewtonsoftJsonGuidId.New();

            var serializedFoo = NewtonsoftJsonSerializer.SerializeObject(foo);
            var serializedGuid = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

            Assert.Equal(serializedFoo, serializedGuid);
        }

        [Fact]
        public void CanSerializeToGuid_WithSystemTextJsonProvider()
        {
            var foo = SystemTextJsonGuidId.New();

            var serializedFoo = SystemTextJsonSerializer.Serialize(foo);
            var serializedGuid = SystemTextJsonSerializer.Serialize(foo.Value);

            Assert.Equal(serializedFoo, serializedGuid);
        }

        [Fact]
        public void CanDeserializeFromGuid_WithNewtonsoftJsonProvider()
        {
            var value = Guid.NewGuid();
            var foo = new NewtonsoftJsonGuidId(value);
            var serializedGuid = NewtonsoftJsonSerializer.SerializeObject(value);

            var deserializedFoo = NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonGuidId>(serializedGuid);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanDeserializeFromGuid_WithSystemTextJsonProvider()
        {
            var value = Guid.NewGuid();
            var foo = new SystemTextJsonGuidId(value);
            var serializedGuid = SystemTextJsonSerializer.Serialize(value);

            var deserializedFoo = SystemTextJsonSerializer.Deserialize<SystemTextJsonGuidId>(serializedGuid);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanSerializeToGuid_WithBothJsonConverters()
        {
            var foo = BothJsonGuidId.New();

            var serializedFoo1 = NewtonsoftJsonSerializer.SerializeObject(foo);
            var serializedGuid1 = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

            var serializedFoo2 = SystemTextJsonSerializer.Serialize(foo);
            var serializedGuid2 = SystemTextJsonSerializer.Serialize(foo.Value);

            Assert.Equal(serializedFoo1, serializedGuid1);
            Assert.Equal(serializedFoo2, serializedGuid2);
        }

        [Fact]
        public void WhenNoJsonConverter_SystemTextJsonSerializesWithValueProperty()
        {
            var foo = NoJsonGuidId.New();

            var serialized = SystemTextJsonSerializer.Serialize(foo);

            var expected = "{\"Value\":\"" + foo.Value + "\"}";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
        {
            var foo = NoJsonGuidId.New();

            var serialized = NewtonsoftJsonSerializer.SerializeObject(foo);

            var expected =  $"\"{foo.Value}\"";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoTypeConverter_SerializesWithValueProperty()
        {
            var foo = NoConverterGuidId.New();

            var newtonsoft = SystemTextJsonSerializer.Serialize(foo);
            var systemText = SystemTextJsonSerializer.Serialize(foo);

            var expected = "{\"Value\":\"" + foo.Value + "\"}";

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
                    new TestEntity { Id = EfCoreGuidId.New() });
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

            var results = await connection.QueryAsync<DapperGuidId>("SELECT '5640dad4-862a-4738-9e3c-c76dc227eb66'");

            var value = Assert.Single(results);
            Assert.Equal(value, new DapperGuidId(Guid.Parse("5640dad4-862a-4738-9e3c-c76dc227eb66")));
        }

        [Theory]
        [InlineData("78104553-f1cd-41ec-bcb6-d3a8ff8d994d")]
        public void TypeConverter_CanConvertToAndFrom(string value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(NoJsonGuidId));
            var id = converter.ConvertFrom(value);
            Assert.IsType<NoJsonGuidId>(id);
            Assert.Equal(new NoJsonGuidId(Guid.Parse(value)), id);

            var reconverted = converter.ConvertTo(id, value.GetType());
            Assert.Equal(value, reconverted);
        }

        [Fact]
        public void CanCompareDefaults()
        {
            ComparableGuidId original = default;
            var other = ComparableGuidId.Empty;

            var compare1 = original.CompareTo(other);
            var compare2 = other.CompareTo(original);
            Assert.Equal(compare1, -compare2);
        }

        [Fact]
        public void CanEquateDefaults()
        {
            EquatableGuidId original = default;
            var other = EquatableGuidId.Empty;

            var equals1 = (original as IEquatable<EquatableGuidId>).Equals(other);
            var equals2 = (other as IEquatable<EquatableGuidId>).Equals(original);

            Assert.Equal(equals1, equals2);
        }

        [Fact]
        public void ImplementsInterfaces()
        {
            Assert.IsAssignableFrom<IEquatable<BothGuidId>>(BothGuidId.Empty);
            Assert.IsAssignableFrom<IComparable<BothGuidId>>(BothGuidId.Empty);

            Assert.IsAssignableFrom<IEquatable<EquatableGuidId>>(EquatableGuidId.Empty);
            Assert.IsAssignableFrom<IComparable<ComparableGuidId>>(ComparableGuidId.Empty);

#pragma warning disable 184
            Assert.False(GuidId1.Empty is IComparable<GuidId1>);
            Assert.False(GuidId1.Empty is IEquatable<GuidId1>);
#pragma warning restore 184
        }

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
                            .HasConversion(new EfCoreGuidId.EfCoreValueConverter())
                            .ValueGeneratedNever();
                    });
            }
        }

        public class TestEntity
        {
            public EfCoreGuidId Id { get; set; }
        }
    }
}