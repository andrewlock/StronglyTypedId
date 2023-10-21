﻿using System;
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
    public class LongIdTests
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

        [Fact]
        public void CanSerializeToLong_WithNewtonsoftJsonProvider()
        {
            var foo = new NewtonsoftJsonLongId(123);

            var serializedFoo = NewtonsoftJsonSerializer.SerializeObject(foo);
            var serializedLong = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

            Assert.Equal(serializedFoo, serializedLong);
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
        public void CanSerializeToLong_WithSystemTextJsonProvider()
        {
            var foo = new SystemTextJsonLongId(123L);

            var serializedFoo = SystemTextJsonSerializer.Serialize(foo);
            var serializedLong = SystemTextJsonSerializer.Serialize(foo.Value);

            Assert.Equal(serializedFoo, serializedLong);
        }

        [Fact]
        public void CanDeserializeFromLong_WithNewtonsoftJsonProvider()
        {
            var value = 123L;
            var foo = new NewtonsoftJsonLongId(value);
            var serializedLong = NewtonsoftJsonSerializer.SerializeObject(value);

            var deserializedFoo = NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonLongId>(serializedLong);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanDeserializeFromLong_WithSystemTextJsonProvider()
        {
            var value = 123L;
            var foo = new SystemTextJsonLongId(value);
            var serializedLong = SystemTextJsonSerializer.Serialize(value);

            var deserializedFoo = SystemTextJsonSerializer.Deserialize<SystemTextJsonLongId>(serializedLong);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanSerializeToLong_WithBothJsonConverters()
        {
            var foo = new BothJsonLongId(123L);

            var serializedFoo1 = NewtonsoftJsonSerializer.SerializeObject(foo);
            var serializedLong1 = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

            var serializedFoo2 = SystemTextJsonSerializer.Serialize(foo);
            var serializedLong2 = SystemTextJsonSerializer.Serialize(foo.Value);

            Assert.Equal(serializedFoo1, serializedLong1);
            Assert.Equal(serializedFoo2, serializedLong2);
        }

        [Fact]
        public void WhenNoJsonConverter_SystemTextJsonSerializesWithValueProperty()
        {
            var foo = new NoJsonLongId(123);

            var serialized = SystemTextJsonSerializer.Serialize(foo);

            var expected = "{\"Value\":" + foo.Value + "}";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
        {
            var foo = new NoJsonLongId(123);

            var serialized = NewtonsoftJsonSerializer.SerializeObject(foo);

            var expected = $"\"{foo.Value}\"";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoTypeConverter_SerializesWithValueProperty()
        {
            var foo = new NoConverterLongId(123);

            var newtonsoft = SystemTextJsonSerializer.Serialize(foo);
            var systemText = SystemTextJsonSerializer.Serialize(foo);

            var expected = "{\"Value\":" + foo.Value + "}";

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

            var original = new TestEntity { Id = new EfCoreLongId(123) };
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

            var results = await connection.QueryAsync<DapperLongId>("SELECT 123");

            var value = Assert.Single(results);
            Assert.Equal(value, new DapperLongId(123));
        }

        [Theory]
        [InlineData(123L)]
        [InlineData("123")]
        public void TypeConverter_CanConvertToAndFrom(object value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(NoJsonLongId));
            var id = converter.ConvertFrom(value);
            Assert.IsType<NoJsonLongId>(id);
            Assert.Equal(new NoJsonLongId(123), id);

            var reconverted = converter.ConvertTo(id, value.GetType());
            Assert.Equal(value, reconverted);
        }

        [Fact]
        public void CanCompareDefaults()
        {
            ComparableLongId original = default;
            var other = ComparableLongId.Empty;

            var compare1 = original.CompareTo(other);
            var compare2 = other.CompareTo(original);
            Assert.Equal(compare1, -compare2);
        }

        [Fact]
        public void CanEquateDefaults()
        {
            EquatableLongId original = default;
            var other = EquatableLongId.Empty;

            var equals1 = (original as IEquatable<EquatableLongId>).Equals(other);
            var equals2 = (other as IEquatable<EquatableLongId>).Equals(original);

            Assert.Equal(equals1, equals2);
        }

        [Fact]
        public void ImplementsInterfaces()
        {
            Assert.IsAssignableFrom<IEquatable<BothLongId>>(BothLongId.Empty);
            Assert.IsAssignableFrom<IComparable<BothLongId>>(BothLongId.Empty);

            Assert.IsAssignableFrom<IEquatable<EquatableLongId>>(EquatableLongId.Empty);
            Assert.IsAssignableFrom<IComparable<ComparableLongId>>(ComparableLongId.Empty);

#pragma warning disable 184
            Assert.False(LongId.Empty is IComparable<LongId>);
            Assert.False(LongId.Empty is IEquatable<LongId>);
#pragma warning restore 184
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
                    new TestEntity { Id = new EfCoreLongId(123) });
                context.SaveChanges();
            }
            using (var context = new ConventionsDbContext(options))
            {
                var all = context.Entities.ToList();
                Assert.Single(all);
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
                    .Properties<EfCoreLongId>()
                    .HaveConversion<EfCoreLongId.EfCoreValueConverter>();
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

#if NET5_0_OR_GREATER
        [Fact]
        public void CanShowImplementationTypeExample_WithSwaggerSchemaFilter()
        {
            var schemaGenerator = new Swashbuckle.AspNetCore.SwaggerGen.SchemaGenerator(
                new Swashbuckle.AspNetCore.SwaggerGen.SchemaGeneratorOptions(),
                new Swashbuckle.AspNetCore.SwaggerGen.JsonSerializerDataContractResolver(
                    new System.Text.Json.JsonSerializerOptions()));
            var provider = Microsoft.Extensions.DependencyInjection.ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(
                new Microsoft.Extensions.DependencyInjection.ServiceCollection());
            var schemaFilter = new Swashbuckle.AspNetCore.Annotations.AnnotationsSchemaFilter(provider);
            var schemaRepository = new Swashbuckle.AspNetCore.SwaggerGen.SchemaRepository();

            var idType = typeof(SwaggerLongId);
            var schema = schemaGenerator.GenerateSchema(idType, schemaRepository);
            schemaFilter.Apply(schema, new Swashbuckle.AspNetCore.SwaggerGen.SchemaFilterContext(idType, schemaGenerator, schemaRepository));

            Assert.Equal("integer", schema.Type);
            Assert.Equal("int64", schema.Format);
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
                            .Property(x => x.Id)
                            .HasConversion(new EfCoreLongId.EfCoreValueConverter())
                            .ValueGeneratedNever();
                    });
            }
        }

        public class TestEntity
        {
            public EfCoreLongId Id { get; set; }
        }

        public class EntityWithNullableId
        {
            public NewtonsoftJsonLongId? Id { get; set; }
        }
    }
}
