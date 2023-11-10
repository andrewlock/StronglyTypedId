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

[StronglyTypedId(Template.Guid, "guid-efcore", "guid-dapper", "guid-newtonsoftjson")]
partial struct ConvertersGuidId2 { }

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

[StronglyTypedId("newid-full")]
partial struct NewIdId1 { }

[StronglyTypedId("newid-full")]
partial struct NewIdId2 { }

[StronglyTypedId(Template.String)]
partial struct StringId { }

[StronglyTypedId("string-full")]
partial struct ConvertersStringId { }

[StronglyTypedId("nullablestring-full")]
partial struct NullableStringId { }

public partial class SomeType<T> where T : new()
{
    public partial record struct NestedType<TKey, TValue>
    {
        public partial struct MoreNesting
        {
            [StronglyTypedId]
            public readonly partial struct VeryNestedId
            {
            }
        }
    }
}

