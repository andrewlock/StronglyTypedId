#nullable enable
using System;

namespace StronglyTypedIds.IntegrationTests.Types;


[StronglyTypedId]
partial struct DefaultId1 { }

[StronglyTypedId]
public partial struct DefaultId2 { }

[StronglyTypedId(Template.Guid)]
partial struct GuidId1 { }

[StronglyTypedId("guid-full")]
partial struct ConvertersGuidId { }

[StronglyTypedId(Template.Guid)]
public partial struct GuidId2 { }

[StronglyTypedId(Template.Int)]
partial struct IntId { }

[StronglyTypedId("int-full")]
partial struct ConvertersIntId { }

[StronglyTypedId(Template.Long)]
partial struct LongId { }

[StronglyTypedId("long-full")]
partial struct ConvertersLongId { }
    
[StronglyTypedId("newid")]
partial struct NewIdId1 { }

[StronglyTypedId("newid")]
public partial struct NewIdId2 { }

[StronglyTypedId(Template.String)]
partial struct StringId
{
    public bool TryFormat2(
        global::System.Span<char> destination,
        out int charsWritten,
        global::System.ReadOnlySpan<char> format = default)
    {
        if (destination.Length > Value.Length)
        {
            MemoryExtensions.AsSpan(Value).CopyTo(destination);
            charsWritten = Value.Length;
            return true;
        }
            
        charsWritten = default;
        return false;
    }
}

[StronglyTypedId("string-full")]
partial struct ConvertersStringId { }

[StronglyTypedId(Template.NullableString)]
partial struct NullableStringId { }

[StronglyTypedId("nullablestring-full")]
partial struct ConvertersNullableStringId { }


// public partial class SomeType<T> where T : new()
// {
//     public partial record struct NestedType<TKey, TValue>
//     {
//         public partial struct MoreNesting
//         {
//             [StronglyTypedId]
//             public readonly partial struct VeryNestedIds
//             {
//             }
//         }
//     }
// }
//
