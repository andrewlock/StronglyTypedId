using System;
using System.ComponentModel;
using System.Linq;
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
        public void CanSerializeToNewId_WithTypeConverter()
        {
            var foo = NewtonsoftJsonNewIdId.New();

            var serializedFoo = NewtonsoftJsonSerializer.SerializeObject(foo);
            var serializedNewId = NewtonsoftJsonSerializer.SerializeObject(foo.Value.ToGuid());

            Assert.Equal(serializedFoo, serializedNewId);
        }

        [Fact]
        public void CanSerializeToNewId_WithSystemTextJsonProvider()
        {
            var foo = SystemTextJsonNewIdId.New();

            var serializedFoo = SystemTextJsonSerializer.Serialize(foo);
            var serializedNewId = SystemTextJsonSerializer.Serialize(foo.Value.ToGuid().ToString());

            Assert.Equal(serializedFoo, serializedNewId);
        }

        [Fact]
        public void CanDeserializeFromNewId_WithNewtonsoftJsonProvider()
        {
            var value = NewId.Next();
            var foo = new NewtonsoftJsonNewIdId(value);
            var serializedNewId = NewtonsoftJsonSerializer.SerializeObject(value.ToGuid());

            var deserializedFoo = NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonNewIdId>(serializedNewId);

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
            var foo = new SystemTextJsonNewIdId(value);
            var serializedNewId = SystemTextJsonSerializer.Serialize(value.ToGuid());

            var deserializedFoo = SystemTextJsonSerializer.Deserialize<SystemTextJsonNewIdId>(serializedNewId);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanSerializeToNewId_WithBothJsonConverters()
        {
            var foo = BothJsonNewIdId.New();

            var serializedFoo1 = NewtonsoftJsonSerializer.SerializeObject(foo);
            var serializedNewId1 = NewtonsoftJsonSerializer.SerializeObject(foo.Value.ToGuid().ToString());

            var serializedFoo2 = SystemTextJsonSerializer.Serialize(foo);
            var serializedNewId2 = SystemTextJsonSerializer.Serialize(foo.Value.ToGuid().ToString());

            Assert.Equal(serializedFoo1, serializedNewId1);
            Assert.Equal(serializedFoo2, serializedNewId2);
        }

        [Fact]
        public void WhenNoJsonConverter_SystemTextJsonSerializesWithValueProperty()
        {
            var foo = NoJsonNewIdId.New();

            var serialized = SystemTextJsonSerializer.Serialize(foo);

            var expected = "{\"Value\":" + SystemTextJsonSerializer.Serialize(foo.Value) + "}";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
        {
            var foo = NoJsonNewIdId.New();

            var serialized = NewtonsoftJsonSerializer.SerializeObject(foo);

            var expected =  $"\"{foo.Value.ToGuid()}\"";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoTypeConverter_SerializesWithValueProperty()
        {
            var foo = NoConverterNewIdId.New();

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
                    new TestEntity { Id = EfCoreNewIdId.New() });
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

            var results = await connection.QueryAsync<DapperNewIdId>("SELECT '5640dad4-862a-4738-9e3c-c76dc227eb66'");

            var value = Assert.Single(results);
            Assert.Equal(new DapperNewIdId(NewId.FromGuid(Guid.Parse("5640dad4-862a-4738-9e3c-c76dc227eb66"))), value);
        }

        [Theory]
        [InlineData("78104553-f1cd-41ec-bcb6-d3a8ff8d994d")]
        public void TypeConverter_CanConvertToAndFrom(string value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(NoJsonNewIdId));
            var id = converter.ConvertFrom(value);
            Assert.IsType<NoJsonNewIdId>(id);
            Assert.Equal(new NoJsonNewIdId(NewId.FromGuid(Guid.Parse(value))), id);

            var reconverted = converter.ConvertTo(id, value.GetType());
            Assert.Equal(value, reconverted);
        }

        [Fact]
        public void CanCompareDefaults()
        {
            ComparableNewIdId original = default;
            var other = ComparableNewIdId.Empty;

            var compare1 = original.CompareTo(other);
            var compare2 = other.CompareTo(original);
            Assert.Equal(compare1, -compare2);
        }

        [Fact]
        public void CanEquateDefaults()
        {
            EquatableNewIdId original = default;
            var other = EquatableNewIdId.Empty;

            var equals1 = (original as IEquatable<EquatableNewIdId>).Equals(other);
            var equals2 = (other as IEquatable<EquatableNewIdId>).Equals(original);

            Assert.Equal(equals1, equals2);
        }

        [Fact]
        public void ImplementsInterfaces()
        {
            Assert.IsAssignableFrom<IEquatable<BothNewIdId>>(BothNewIdId.Empty);
            Assert.IsAssignableFrom<IComparable<BothNewIdId>>(BothNewIdId.Empty);

            Assert.IsAssignableFrom<IEquatable<EquatableNewIdId>>(EquatableNewIdId.Empty);
            Assert.IsAssignableFrom<IComparable<ComparableNewIdId>>(ComparableNewIdId.Empty);

#pragma warning disable 184
            Assert.False(NewIdId1.Empty is IComparable<NewIdId1>);
            Assert.False(NewIdId1.Empty is IEquatable<NewIdId1>);
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
                            .HasConversion(new EfCoreNewIdId.EfCoreValueConverter())
                            .ValueGeneratedNever();
                    });
            }
        }

        public class TestEntity
        {
            public EfCoreNewIdId Id { get; set; }
        }

        public class EntityWithNullableId
        {
            public NewtonsoftJsonNewIdId? Id { get; set; }
        }
    }
}