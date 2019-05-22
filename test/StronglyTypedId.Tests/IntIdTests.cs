using Newtonsoft.Json;
using Xunit;

namespace StronglyTypedId
{
    public class IntIdTests
    {
        [Fact]
        public void SameValuesAreEqual()
        {
            var id = 123;
            var foo1 = new IntId(id);
            var foo2 = new IntId(id);

            Assert.Equal(foo1, foo2);
        }

        [Fact]
        public void EmptyValueIsEmpty()
        {
            Assert.Equal(0, IntId.Empty.Value);
        }


        [Fact]
        public void DifferentValuesAreUnequal()
        {
            var foo1 = new IntId(1);
            var foo2 = new IntId(2);

            Assert.NotEqual(foo1, foo2);
        }

        [Fact]
        public void OverloadsWorkCorrectly()
        {
            var id = 12;
            var same1 = new IntId(id);
            var same2 = new IntId(id);
            var different = new IntId(3);

            Assert.True(same1 == same2);
            Assert.False(same1 == different);
            Assert.False(same1 != same2);
            Assert.True(same1 != different);
        }

        [Fact]
        public void DifferentTypesAreUnequal()
        {
            var bar = GeneratedId2.New();
            var foo = new IntId(23);

            //Assert.NotEqual(bar, foo); // does not compile
            Assert.NotEqual((object)bar, (object)foo);
        }

        [Fact]
        public void CanSerializeToString()
        {
            var value = 123;
            var foo = new IntId(value);

            var serializedFoo = JsonConvert.SerializeObject(foo);
            var serializedString = JsonConvert.SerializeObject(value);

            Assert.Equal(serializedFoo, serializedString);
        }

        [Fact]
        public void CanSerializeFromString()
        {
            var value = 123;
            var foo = new IntId(value);

            var serializedValue = JsonConvert.SerializeObject(value);
            var deserializedFoo = JsonConvert.DeserializeObject<IntId>(serializedValue);

            Assert.Equal(foo, deserializedFoo);
        }


        [Fact]
        public void WhenNoJsonConverter_SerializesWithValueProperty()
        {
            var foo = new NoJsonIntId(123);

            var serialized = JsonConvert.SerializeObject(foo);
            var expected = "{\"Value\":123}";

            Assert.Equal(expected, serialized);
        }

    }
}