// using StronglyTypedId.Tests.Types;
// using Xunit;
// using NewtonsoftJsonSerializer = Newtonsoft.Json.JsonConvert;
// using SystemTextJsonSerializer = System.Text.Json.JsonSerializer;
//
// namespace StronglyTypedId.Tests
// {
//     public class IntIdTests
//     {
//         [Fact]
//         public void SameValuesAreEqual()
//         {
//             var id = 123;
//             var foo1 = new IntId(id);
//             var foo2 = new IntId(id);
//
//             Assert.Equal(foo1, foo2);
//         }
//
//         [Fact]
//         public void EmptyValueIsEmpty()
//         {
//             Assert.Equal(0, IntId.Empty.Value);
//         }
//
//
//         [Fact]
//         public void DifferentValuesAreUnequal()
//         {
//             var foo1 = new IntId(1);
//             var foo2 = new IntId(2);
//
//             Assert.NotEqual(foo1, foo2);
//         }
//
//         [Fact]
//         public void OverloadsWorkCorrectly()
//         {
//             var id = 12;
//             var same1 = new IntId(id);
//             var same2 = new IntId(id);
//             var different = new IntId(3);
//
//             Assert.True(same1 == same2);
//             Assert.False(same1 == different);
//             Assert.False(same1 != same2);
//             Assert.True(same1 != different);
//         }
//
//         [Fact]
//         public void DifferentTypesAreUnequal()
//         {
//             var bar = GuidId2.New();
//             var foo = new IntId(23);
//
//             //Assert.NotEqual(bar, foo); // does not compile
//             Assert.NotEqual((object)bar, (object)foo);
//         }
//
//         [Fact]
//         public void CanSerializeToInt_WithNewtonsoftJsonProvider()
//         {
//             var foo = new NewtonsoftJsonIntId(123);
//
//             var serializedFoo = NewtonsoftJsonSerializer.SerializeObject(foo);
//             var serializedInt = NewtonsoftJsonSerializer.SerializeObject(foo.Value);
//
//             Assert.Equal(serializedFoo, serializedInt);
//         }
//
//         [Fact]
//         public void CanSerializeToInt_WithSystemTextJsonProvider()
//         {
//             var foo = new SystemTextJsonIntId(123);
//
//             var serializedFoo = SystemTextJsonSerializer.Serialize(foo);
//             var serializedInt = SystemTextJsonSerializer.Serialize(foo.Value);
//
//             Assert.Equal(serializedFoo, serializedInt);
//         }
//
//         [Fact]
//         public void CanDeserializeFromInt_WithNewtonsoftJsonProvider()
//         {
//             var value = 123;
//             var foo = new NewtonsoftJsonIntId(value);
//             var serializedInt = NewtonsoftJsonSerializer.SerializeObject(value);
//
//             var deserializedFoo = NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonIntId>(serializedInt);
//
//             Assert.Equal(foo, deserializedFoo);
//         }
//
//         [Fact]
//         public void CanDeserializeFromInt_WithSystemTextJsonProvider()
//         {
//             var value = 123;
//             var foo = new SystemTextJsonIntId(value);
//             var serializedInt = SystemTextJsonSerializer.Serialize(value);
//
//             var deserializedFoo = SystemTextJsonSerializer.Deserialize<SystemTextJsonIntId>(serializedInt);
//
//             Assert.Equal(foo, deserializedFoo);
//         }
//
//         [Fact]
//         public void CanSerializeToInt_WithBothJsonConverters()
//         {
//             var foo = new BothJsonIntId(123);
//
//             var serializedFoo1 = NewtonsoftJsonSerializer.SerializeObject(foo);
//             var serializedInt1 = NewtonsoftJsonSerializer.SerializeObject(foo.Value);
//
//             var serializedFoo2 = SystemTextJsonSerializer.Serialize(foo);
//             var serializedInt2 = SystemTextJsonSerializer.Serialize(foo.Value);
//
//             Assert.Equal(serializedFoo1, serializedInt1);
//             Assert.Equal(serializedFoo2, serializedInt2);
//         }
//
//         [Fact]
//         public void WhenNoJsonConverter_SerializesWithValueProperty()
//         {
//             var foo = new NoJsonIntId(123);
//
//             var serialized1 = NewtonsoftJsonSerializer.SerializeObject(foo);
//             var serialized2 = SystemTextJsonSerializer.Serialize(foo);
//
//             var expected = "{\"Value\":" + foo.Value + "}";
//
//             Assert.Equal(expected, serialized1);
//             Assert.Equal(expected, serialized2);
//         }
//     }
// }