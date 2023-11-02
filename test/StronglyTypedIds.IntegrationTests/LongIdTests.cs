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
            var foo = new ConvertersLongId(123);

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
        public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
        {
            var foo = new LongId(123);

            var serialized = NewtonsoftJsonSerializer.SerializeObject(foo);

            var expected = $"\"{foo.Value}\"";

            Assert.Equal(expected, serialized);
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

        internal class ConventionsDbContext : DbContext
        {
            public DbSet<TestEntity> Entities { get; set; }

            public ConventionsDbContext(DbContextOptions options) : base(options)
            {
            }

            protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
            {
                configurationBuilder
                    .Properties<ConvertersLongId>()
                    .HaveConversion<ConvertersLongId.EfCoreValueConverter>();
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
                            .HasConversion(new ConvertersLongId.EfCoreValueConverter())
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
    }
}