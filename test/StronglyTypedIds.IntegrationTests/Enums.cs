#nullable enable
using System;

namespace StronglyTypedIds.IntegrationTests.Types;


[StronglyTypedId]
internal partial struct DefaultId1 { }

[StronglyTypedId]
internal partial struct DefaultId2 { }

[StronglyTypedId(Template.Guid)]
internal partial struct GuidId1 { }

[StronglyTypedId("guid-full")]
internal partial struct ConvertersGuidId { }

[StronglyTypedId(Template.Guid, "guid-efcore", "guid-dapper", "guid-newtonsoftjson")]
internal partial struct ConvertersGuidId2 { }

[StronglyTypedId(Template.Guid)]
internal partial struct GuidId2 { }

[StronglyTypedId(Template.Int)]
internal partial struct IntId { }

[StronglyTypedId("int-full")]
internal partial struct ConvertersIntId { }

[StronglyTypedId(Template.Int, "int-efcore", "int-dapper", "int-newtonsoftjson")]
internal partial struct ConvertersIntId2 { }

[StronglyTypedId(Template.Long)]
internal partial struct LongId { }

[StronglyTypedId("long-full")]
internal partial struct ConvertersLongId { }

[StronglyTypedId(Template.Long, "long-efcore", "long-dapper", "long-newtonsoftjson")]
internal partial struct ConvertersLongId2 { }

[StronglyTypedId("newid-full")]
internal partial struct NewIdId1 { }

[StronglyTypedId("newid-full")]
internal partial struct NewIdId2 { }

[StronglyTypedId(Template.String)]
internal partial struct StringId { }

[StronglyTypedId("string-full")]
internal partial struct ConvertersStringId { }

[StronglyTypedId(Template.String, "string-efcore", "string-dapper", "string-newtonsoftjson")]
internal partial struct ConvertersStringId2 { }

[StronglyTypedId("nullablestring-full")]
internal partial struct NullableStringId { }

// Note, must be public, otherwise JsonSerializerContext can't find required public constructor
[StronglyTypedId]
public partial class ClassGuidId { }

internal partial class SomeType<T> where T : new()
{
    internal partial record struct NestedType<TKey, TValue>
    {
        internal partial struct MoreNesting
        {
            [StronglyTypedId]
            internal readonly partial struct VeryNestedId
            {
            }
        }
    }
}

