using System;
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
    }
}