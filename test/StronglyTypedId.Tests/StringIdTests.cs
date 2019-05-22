using System;
using Newtonsoft.Json;
using Xunit;

namespace StronglyTypedId
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
            var bar = GeneratedId2.New();
            var foo = new StringId("Value");

            //Assert.NotEqual(bar, foo); // does not compile
            Assert.NotEqual((object)bar, (object)foo);
        }
        
        [Fact]
        public void CanSerializeToString()
        {
            var value = "value1";
            var foo = new StringId(value);

            var serializedFoo = JsonConvert.SerializeObject(foo);
            var serializedString = JsonConvert.SerializeObject(value);

            Assert.Equal(serializedFoo, serializedString);
        }

        [Fact]
        public void CanSerializeFromString()
        {
            var value = "value1";
            var foo = new StringId(value);

            var serializedValue = JsonConvert.SerializeObject(value);
            var deserializedFoo = JsonConvert.DeserializeObject<StringId>(serializedValue);

            Assert.Equal(foo, deserializedFoo);
        }


        [Fact]
        public void WhenNoJsonConverter_SerializesWithValueProperty()
        {
            var foo = new NoJsonStringId("the value");

            var serialized = JsonConvert.SerializeObject(foo);
            var expected = "{\"Value\":\"the value\"}";

            Assert.Equal(expected, serialized);
        }

    }
}