using StronglyTypedIds.IntegrationTests.Types;
using Xunit;
using NewtonsoftJsonSerializer = Newtonsoft.Json.JsonConvert;
using SystemTextJsonSerializer = System.Text.Json.JsonSerializer;

namespace StronglyTypedIds.IntegrationTests
{
    public class StringIdTests
    {
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
            var foo = new NewtonsoftJsonStringId("123");

            var serializedFoo = NewtonsoftJsonSerializer.SerializeObject(foo);
            var serializedString = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

            Assert.Equal(serializedFoo, serializedString);
        }

        [Fact]
        public void CanSerializeToString_WithSystemTextJsonProvider()
        {
            var foo = new SystemTextJsonStringId("123");

            var serializedFoo = SystemTextJsonSerializer.Serialize(foo);
            var serializedString = SystemTextJsonSerializer.Serialize(foo.Value);

            Assert.Equal(serializedFoo, serializedString);
        }

        [Fact]
        public void CanDeserializeFromString_WithNewtonsoftJsonProvider()
        {
            var value = "123";
            var foo = new NewtonsoftJsonStringId(value);
            var serializedString = NewtonsoftJsonSerializer.SerializeObject(value);

            var deserializedFoo = NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonStringId>(serializedString);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanDeserializeFromString_WithSystemTextJsonProvider()
        {
            var value = "123";
            var foo = new SystemTextJsonStringId(value);
            var serializedString = SystemTextJsonSerializer.Serialize(value);

            var deserializedFoo = SystemTextJsonSerializer.Deserialize<SystemTextJsonStringId>(serializedString);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanSerializeToString_WithBothJsonConverters()
        {
            var foo = new BothJsonStringId("123");

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
            var foo = new NoJsonStringId("123");

            var serialized = SystemTextJsonSerializer.Serialize(foo);

            var expected = "{\"Value\":\"" + foo.Value + "\"}";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
        {
            var foo = new NoJsonStringId("123");

            var serialized = NewtonsoftJsonSerializer.SerializeObject(foo);

            var expected = "\"" + foo.Value + "\"";

            Assert.Equal(expected, serialized);
        }
    }
}