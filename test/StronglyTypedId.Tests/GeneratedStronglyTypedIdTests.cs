using System;
using Newtonsoft.Json;
using Xunit;

namespace StronglyTypedId
{
    public class GeneratedStronglyTypedIdTests
    {
        [Fact]
        public void SameValuesAreEqual()
        {
            var id = Guid.NewGuid();
            var foo1 = new GeneratedId1(id);
            var foo2 = new GeneratedId1(id);

            Assert.Equal(foo1, foo2);
        }

        [Fact]
        public void EmptyValueIsEmpty()
        {
            Assert.Equal(GeneratedId1.Empty.Value, Guid.Empty);
        }


        [Fact]
        public void DifferentValuesAreUnequal()
        {
            var foo1 = GeneratedId1.New();
            var foo2 = GeneratedId1.New();

            Assert.NotEqual(foo1, foo2);
        }

        [Fact]
        public void OverloadsWorkCorrectly()
        {
            var id = Guid.NewGuid();
            var same1 = new GeneratedId1(id);
            var same2 = new GeneratedId1(id);
            var different = GeneratedId1.New();

            Assert.True(same1 == same2);
            Assert.False(same1 == different);
            Assert.False(same1 != same2);
            Assert.True(same1 != different);
        }

        [Fact]
        public void DifferentTypesAreUnequal()
        {
            var bar = GeneratedId2.New();
            var foo = GeneratedId1.New();

            //Assert.NotEqual(bar, foo); // does not compile
            Assert.NotEqual((object)bar, (object)foo);
        }

        [Fact]
        public void CantCreateEmptyGeneratedId1()
        {
            var foo = new GeneratedId1();
            var bar = new GeneratedId2();

            //Assert.NotEqual(bar, foo); // does not compile
            Assert.NotEqual((object)bar, (object)foo);
        }


        [Fact]
        public void CanSerializeToGuid()
        {
            var foo = GeneratedId1.New();

            var serializedFoo = JsonConvert.SerializeObject(foo);
            var serializedGuid = JsonConvert.SerializeObject(foo.Value);

            Assert.Equal(serializedFoo, serializedGuid);
        }

        [Fact]
        public void CanSerializeFromGuid()
        {
            var value = Guid.NewGuid();
            var foo = new GeneratedId1(value);
            var serializedGuid = JsonConvert.SerializeObject(value);

            var deserializedFoo = JsonConvert.DeserializeObject<GeneratedId1>(serializedGuid);

            Assert.Equal(foo, deserializedFoo);
        }
        
        
        [Fact]
        public void WhenNoJsonConverter_SerializesWithValueProperty()
        {
            var foo = NoJsonGeneratedId.New();

            var serialized = JsonConvert.SerializeObject(foo);
            var expected = "{\"Value\":\"" + foo.Value + "\"}";

            Assert.Equal(expected, serialized);
        }

    }
}