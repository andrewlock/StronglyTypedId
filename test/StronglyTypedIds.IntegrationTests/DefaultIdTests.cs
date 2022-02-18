using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using StronglyTypedIds.IntegrationTests.Types;
using Xunit;
using NewtonsoftJsonSerializer = Newtonsoft.Json.JsonConvert;
using SystemTextJsonSerializer = System.Text.Json.JsonSerializer;

namespace StronglyTypedIds.IntegrationTests
{
    public class DefaultIdTests
    {
        [Fact]
        public void SameValuesAreEqual()
        {
            var id = Guid.NewGuid();
            var foo1 = new DefaultId1(id);
            var foo2 = new DefaultId1(id);

            Assert.Equal(foo1, foo2);
        }

        [Fact]
        public void EmptyValueIsEmpty()
        {
            Assert.Equal(DefaultId1.Empty.Value, Guid.Empty);
        }

        [Fact]
        public void DifferentValuesAreUnequal()
        {
            var foo1 = DefaultId1.New();
            var foo2 = DefaultId1.New();

            Assert.NotEqual(foo1, foo2);
        }

        [Fact]
        public void OverloadsWorkCorrectly()
        {
            var id = Guid.NewGuid();
            var same1 = new DefaultId1(id);
            var same2 = new DefaultId1(id);
            var different = DefaultId1.New();

            Assert.True(same1 == same2);
            Assert.False(same1 == different);
            Assert.False(same1 != same2);
            Assert.True(same1 != different);
        }

        [Fact]
        public void DifferentTypesAreUnequal()
        {
            var bar = DefaultId2.New();
            var foo = DefaultId1.New();

            //Assert.NotEqual(bar, foo); // does not compile
            Assert.NotEqual((object)bar, (object)foo);
        }

        [Fact]
        public void CantCreateEmptyGeneratedId1()
        {
            var foo = new DefaultId1();
            var bar = new DefaultId2();

            //Assert.NotEqual(bar, foo); // does not compile
            Assert.NotEqual((object)bar, (object)foo);
        }

        [Fact]
        public void WhenNoTypeConverter_SerializesWithValueProperty()
        {
            var foo = DefaultId1.New();

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
                    new TestEntity { Id = EfCoreDefaultId.New() });
                context.SaveChanges();
            }
            using (var context = new TestDbContext(options))
            {
                var all = context.Entities.ToList();
                Assert.Single(all);
            }
        }

        [Theory]
        [InlineData("78104553-f1cd-41ec-bcb6-d3a8ff8d994d")]
        public void TypeConverter_CanConvertToAndFrom(string value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(NoJsonDefaultId));
            var id = converter.ConvertFrom(value);
            Assert.IsType<NoJsonDefaultId>(id);
            Assert.Equal(new NoJsonDefaultId(Guid.Parse(value)), id);

            var reconverted = converter.ConvertTo(id, value.GetType());
            Assert.Equal(value, reconverted);
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
                    new TestEntity { Id = EfCoreDefaultId.New() });
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
                    .Properties<EfCoreDefaultId>()
                    .HaveConversion<EfCoreDefaultId.EfCoreValueConverter>();
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
                            .Property(x => x.Id)
                            .HasConversion(new EfCoreDefaultId.EfCoreValueConverter())
                            .ValueGeneratedNever();
                    });
            }
        }

        public class TestEntity
        {
            public EfCoreDefaultId Id { get; set; }
        }
    }
}