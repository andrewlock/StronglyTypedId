using System;
using StronglyTypedIds.IntegrationTests.Types;
using Xunit;
using NewtonsoftJsonSerializer = Newtonsoft.Json.JsonConvert;
using SystemTextJsonSerializer = System.Text.Json.JsonSerializer;

namespace StronglyTypedIds.IntegrationTests
{
    public class GuidIdTests
    {
        [Fact]
        public void SameValuesAreEqual()
        {
            var id = Guid.NewGuid();
            var foo1 = new GuidId1(id);
            var foo2 = new GuidId1(id);

            Assert.Equal(foo1, foo2);
        }

        [Fact]
        public void EmptyValueIsEmpty()
        {
            Assert.Equal(GuidId1.Empty.Value, Guid.Empty);
        }


        [Fact]
        public void DifferentValuesAreUnequal()
        {
            var foo1 = GuidId1.New();
            var foo2 = GuidId1.New();

            Assert.NotEqual(foo1, foo2);
        }

        [Fact]
        public void OverloadsWorkCorrectly()
        {
            var id = Guid.NewGuid();
            var same1 = new GuidId1(id);
            var same2 = new GuidId1(id);
            var different = GuidId1.New();

            Assert.True(same1 == same2);
            Assert.False(same1 == different);
            Assert.False(same1 != same2);
            Assert.True(same1 != different);
        }

        [Fact]
        public void DifferentTypesAreUnequal()
        {
            var bar = GuidId2.New();
            var foo = GuidId1.New();

            //Assert.NotEqual(bar, foo); // does not compile
            Assert.NotEqual((object)bar, (object)foo);
        }

        [Fact]
        public void CantCreateEmptyGeneratedId1()
        {
            var foo = new GuidId1();
            var bar = new GuidId2();

            //Assert.NotEqual(bar, foo); // does not compile
            Assert.NotEqual((object)bar, (object)foo);
        }


        [Fact]
        public void CanSerializeToGuid_WithNewtonsoftJsonProvider()
        {
            var foo = NewtonsoftJsonGuidId.New();

            var serializedFoo = NewtonsoftJsonSerializer.SerializeObject(foo);
            var serializedGuid = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

            Assert.Equal(serializedFoo, serializedGuid);
        }

        [Fact]
        public void CanSerializeToGuid_WithSystemTextJsonProvider()
        {
            var foo = SystemTextJsonGuidId.New();

            var serializedFoo = SystemTextJsonSerializer.Serialize(foo);
            var serializedGuid = SystemTextJsonSerializer.Serialize(foo.Value);

            Assert.Equal(serializedFoo, serializedGuid);
        }

        [Fact]
        public void CanDeserializeFromGuid_WithNewtonsoftJsonProvider()
        {
            var value = Guid.NewGuid();
            var foo = new NewtonsoftJsonGuidId(value);
            var serializedGuid = NewtonsoftJsonSerializer.SerializeObject(value);

            var deserializedFoo = NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonGuidId>(serializedGuid);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanDeserializeFromGuid_WithSystemTextJsonProvider()
        {
            var value = Guid.NewGuid();
            var foo = new SystemTextJsonGuidId(value);
            var serializedGuid = SystemTextJsonSerializer.Serialize(value);

            var deserializedFoo = SystemTextJsonSerializer.Deserialize<SystemTextJsonGuidId>(serializedGuid);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanSerializeToGuid_WithBothJsonConverters()
        {
            var foo = BothJsonGuidId.New();

            var serializedFoo1 = NewtonsoftJsonSerializer.SerializeObject(foo);
            var serializedGuid1 = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

            var serializedFoo2 = SystemTextJsonSerializer.Serialize(foo);
            var serializedGuid2 = SystemTextJsonSerializer.Serialize(foo.Value);

            Assert.Equal(serializedFoo1, serializedGuid1);
            Assert.Equal(serializedFoo2, serializedGuid2);
        }

        [Fact]
        public void WhenNoJsonConverter_SerializesWithValueProperty()
        {
            var foo = NoJsonGuidId.New();

            var serialized1 = NewtonsoftJsonSerializer.SerializeObject(foo);
            var serialized2 = SystemTextJsonSerializer.Serialize(foo);

            var expected = "{\"Value\":\"" + foo.Value + "\"}";

            Assert.Equal(expected, serialized1);
            Assert.Equal(expected, serialized2);
        }
    }
}