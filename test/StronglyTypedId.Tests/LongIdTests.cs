using StronglyTypedId.Tests.Types;
using Xunit;
using NewtonsoftJsonSerializer = Newtonsoft.Json.JsonConvert;
using SystemTextJsonSerializer = System.Text.Json.JsonSerializer;

namespace StronglyTypedId.Tests
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
        public void WhenNoJsonConverter_SerializesWithValueProperty()
        {
            var foo = new NoJsonLongId(123L);

            var serialized1 = NewtonsoftJsonSerializer.SerializeObject(foo);
            var serialized2 = SystemTextJsonSerializer.Serialize(foo);

            var expected = "{\"Value\":" + foo.Value + "}";

            Assert.Equal(expected, serialized1);
            Assert.Equal(expected, serialized2);
        }
    }
}